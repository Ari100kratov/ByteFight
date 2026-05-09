using Domain.GameRuntime.GameActionLogs;
using Domain.ValueObjects;
using GameRuntime.Common;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Abilities;
using GameRuntime.Common.World.Units;
using GameRuntime.Logic.Actions;
using GameRuntime.Logic.NPC.PathFinding;
using GameRuntime.Logic.Turns;

namespace GameRuntime.Logic.NPC;

internal sealed class BasicEnemyAiProcessor : IUnitTurnProcessor
{
    private readonly IPathFinder pathFinder;

    public BasicEnemyAiProcessor(IPathFinder pathFinder)
    {
        this.pathFinder = pathFinder;
    }

    public IEnumerable<GameActionLogEntry> ProcessTurn(BaseUnit actor, ArenaWorld world)
    {
        RuntimeAbility? healingAbility = actor.Abilities.FindBestHealingAbility();

        if (healingAbility is not null && !IsLastAliveEnemy(world))
        {
            IEnumerable<GameActionLogEntry>? healingAction =
                TryCreateHealingAction(actor, world, healingAbility);

            if (healingAction is not null)
            {
                return healingAction;
            }
        }

        int distanceToPlayer = actor.Position.ManhattanDistance(world.Player.Position);

        RuntimeAbility? attack = actor.Abilities.FindBestBasicAttack(distanceToPlayer);

        if (attack is not null)
        {
            return new UseAbilityAction(actor, world.Player, attack).Execute(world);
        }

        return MoveToTarget(actor, world, world.Player.Position);
    }

    private static bool IsLastAliveEnemy(ArenaWorld world)
    {
        return world.Enemies.Count(x => !x.IsDead) == 1;
    }

    private IEnumerable<GameActionLogEntry>? TryCreateHealingAction(
        BaseUnit actor,
        ArenaWorld world,
        RuntimeAbility healingAbility)
    {
        if (NeedsHealing(actor))
        {
            int selfDistance = 0;

            if (healingAbility.CanReach(selfDistance))
            {
                return new UseAbilityAction(actor, actor, healingAbility).Execute(world);
            }
        }

        EnemyUnit? target = world.Enemies
            .Where(x => !x.IsDead)
            .Where(NeedsHealing)
            .OrderBy(x => x.Stats.GetHealthPercent())
            .ThenBy(x => actor.Position.ManhattanDistance(x.Position))
            .FirstOrDefault();

        if (target is null)
        {
            return null;
        }

        int distance = actor.Position.ManhattanDistance(target.Position);

        if (healingAbility.CanReach(distance))
        {
            return new UseAbilityAction(actor, target, healingAbility).Execute(world);
        }

        return MoveToTarget(actor, world, target.Position);
    }

    private static bool NeedsHealing(BaseUnit unit) => !unit.IsDead && !unit.Stats.IsHealthFull();

    private IEnumerable<GameActionLogEntry> MoveToTarget(
        BaseUnit actor,
        ArenaWorld world,
        Position targetPosition)
    {
        List<Position>? path = pathFinder.FindPath(
            world,
            actor.Position,
            targetPosition);

        if (path is null || path.Count == 0)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.NoPath)];
        }

        int moveRange = actor.Stats.GetMoveRange();

        Position? target = MovementRules.SelectMoveTarget(
            world,
            actor,
            path,
            moveRange);

        if (target is null)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.MoveImpossible)];
        }

        return new MoveAction(actor, target).Execute(world);
    }
}
