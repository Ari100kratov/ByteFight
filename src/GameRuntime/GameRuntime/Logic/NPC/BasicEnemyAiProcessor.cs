using System.Collections.Immutable;
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

internal sealed class BasicEnemyAiProcessor(IPathFinder pathFinder) : IUnitTurnProcessor
{
    public IEnumerable<GameActionLogEntry> ProcessTurn(BaseUnit actor, ArenaWorld world)
    {
        if (actor is not EnemyUnit enemy)
        {
            throw new InvalidOperationException(
                $"{nameof(BasicEnemyAiProcessor)} can process only enemy units.");
        }

        RuntimeAbility? healingAbility = enemy.Abilities.FindBestHealingAbility();

        if (healingAbility is not null && !IsLastAliveEnemy(world))
        {
            IEnumerable<GameActionLogEntry>? defensiveAction =
                TryCreateOneTimeDefensiveHealerAction(enemy, world, healingAbility);

            if (defensiveAction is not null)
            {
                return defensiveAction;
            }

            IEnumerable<GameActionLogEntry>? healingAction =
                TryCreateHealingAction(enemy, world, healingAbility);

            if (healingAction is not null)
            {
                return healingAction;
            }
        }

        int distanceToPlayer = enemy.Position.ManhattanDistance(world.Player.Position);

        RuntimeAbility? attack = enemy.Abilities.FindBestBasicAttack(distanceToPlayer);

        if (attack is not null)
        {
            return new UseAbilityAction(enemy, world.Player, attack).Execute(world);
        }

        return MoveToTarget(enemy, world, world.Player.Position);
    }

    private IEnumerable<GameActionLogEntry>? TryCreateOneTimeDefensiveHealerAction(
        EnemyUnit actor,
        ArenaWorld world,
        RuntimeAbility healingAbility)
    {
        if (!NeedsHealing(actor))
        {
            return null;
        }

        if (!actor.AiState.HasRetreatedAfterBeingHit)
        {
            actor.AiState.MarkRetreatedAfterBeingHit();

            IEnumerable<GameActionLogEntry>? retreatAction =
                TryMoveAwayFromPlayer(actor, world);

            if (retreatAction is not null)
            {
                return retreatAction;
            }

            return TryCreateOneTimeSelfHeal(actor, world, healingAbility);
        }

        return TryCreateOneTimeSelfHeal(actor, world, healingAbility);
    }

    private static IEnumerable<GameActionLogEntry>? TryCreateOneTimeSelfHeal(
        EnemyUnit actor,
        ArenaWorld world,
        RuntimeAbility healingAbility)
    {
        if (actor.AiState.HasSelfHealedAfterBeingHit || !healingAbility.CanReach(0))
        {
            return null;
        }

        actor.AiState.MarkSelfHealedAfterBeingHit();

        return new UseAbilityAction(actor, actor, healingAbility).Execute(world);
    }

    private IEnumerable<GameActionLogEntry>? TryMoveAwayFromPlayer(
        EnemyUnit actor,
        ArenaWorld world)
    {
        int moveRange = actor.Stats.GetMoveRange();

        ImmutableHashSet<Position> reachable = world.GetReachableCells(
            actor,
            actor.Position,
            moveRange);

        Position? bestPosition = reachable
            .Where(position => position != actor.Position)
            .OrderByDescending(position => position.ManhattanDistance(world.Player.Position))
            .ThenBy(position => position.ManhattanDistance(actor.Position))
            .Cast<Position?>()
            .FirstOrDefault();

        if (bestPosition is null)
        {
            return null;
        }

        return new MoveAction(actor, bestPosition).Execute(world);
    }

    private static bool IsLastAliveEnemy(ArenaWorld world)
    {
        return world.Enemies.Count(x => !x.IsDead) == 1;
    }

    private IEnumerable<GameActionLogEntry>? TryCreateHealingAction(
        EnemyUnit actor,
        ArenaWorld world,
        RuntimeAbility healingAbility)
    {
        if (!actor.AiState.HasSelfHealedAfterBeingHit &&
            NeedsHealing(actor) &&
            healingAbility.CanReach(0))
        {
            return new UseAbilityAction(actor, actor, healingAbility).Execute(world);
        }

        EnemyUnit? target = world.Enemies
            .Where(x => !x.IsDead)
            .Where(x => x.Id != actor.Id)
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

    private static bool NeedsHealing(BaseUnit unit)
    {
        return !unit.IsDead && !unit.Stats.IsHealthFull();
    }

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
