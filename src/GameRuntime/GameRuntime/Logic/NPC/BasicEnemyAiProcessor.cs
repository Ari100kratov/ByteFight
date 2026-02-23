using Domain.Game.Stats;
using Domain.GameRuntime.GameActionLogs;
using Domain.ValueObjects;
using GameRuntime.Logic.Actions;
using GameRuntime.Logic.NPC.PathFinding;
using GameRuntime.Logic.Turns;
using GameRuntime.World;
using GameRuntime.World.Units;

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
            return [world.CreateIdleLogEntry(actor.Id, IdleReasons.NoPath)];
        }

        var trimmed = path.Skip(1).ToList();
        if (!trimmed.Any())
        {
            return [world.CreateIdleLogEntry(actor.Id, IdleReasons.MoveImpossible)];
        }

        int moveRange = (int)Math.Floor(actor.Stats.Get(StatType.MoveRange));
        int stepsToMove = Math.Min(moveRange, trimmed.Count);
        Position target = trimmed[stepsToMove - 1];

        var moveAction = new MoveAction(actor, target);
        return moveAction.Execute(world);
    }
}
