using Domain.Game.Stats;
using Domain.GameRuntime.GameActionLogs;
using Domain.ValueObjects;
using GameRuntime.Common;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Units;
using GameRuntime.Logic.Actions;
using GameRuntime.Logic.NPC.PathFinding;
using GameRuntime.Logic.Turns;

namespace GameRuntime.Logic.NPC;

internal sealed class BasicEnemyAiProcessor : IUnitTurnProcessor
{
    private readonly IPathFinder _pathFinder;

    public BasicEnemyAiProcessor(IPathFinder pathFinder)
    {
        _pathFinder = pathFinder;
    }

    public IEnumerable<GameActionLogEntry> ProcessTurn(BaseUnit actor, ArenaWorld world)
    {
        int distance = actor.Position.ManhattanDistance(world.Player.Position);
        int attackRange = (int)Math.Ceiling(actor.Stats.Get(StatType.AttackRange));

        // Если можем атаковать
        if (distance <= attackRange)
        {
            decimal damage = actor.Stats.Get(StatType.Attack);

            var attackAction = new AttackAction(actor, world.Player, damage);
            return attackAction.Execute(world);
        }

        // Иначе пробуем двигаться в сторону персонажа игрока
        List<Position>? path = _pathFinder.FindPath(world, actor.Position, world.Player.Position);

        if (path is null || path.Count == 0)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.NoPath)];
        }

        int moveRange = (int)Math.Floor(actor.Stats.Get(StatType.MoveRange));

        Position? target = MovementRules.SelectMoveTarget(
            world,
            actor,
            path,
            moveRange);

        if (target is null)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.MoveImpossible)];
        }

        var moveAction = new MoveAction(actor, target);
        return moveAction.Execute(world);
    }
}
