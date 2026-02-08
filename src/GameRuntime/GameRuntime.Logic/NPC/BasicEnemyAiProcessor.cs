using Domain.Game.Stats;
using Domain.GameRuntime.RuntimeLogEntries;
using Domain.ValueObjects;
using GameRuntime.Core;
using GameRuntime.Core.Units;
using GameRuntime.Logic.Actions;
using GameRuntime.Logic.NPC.PathFinding;
using GameRuntime.Logic.Turns;

namespace GameRuntime.Logic.NPC;

public sealed class BasicEnemyAiProcessor : IUnitTurnProcessor
{
    private readonly IPathFinder _pathFinder;

    public BasicEnemyAiProcessor(IPathFinder pathFinder)
    {
        _pathFinder = pathFinder;
    }

    public IEnumerable<RuntimeLogEntry> ProcessTurn(BaseUnit actor, ArenaWorld world)
    {
        int distance = actor.Position.ManhattanDistance(world.Player.Position);
        int attackRange = (int)Math.Ceiling(actor.Stats.Get(StatType.AttackRange));

        // Если можем атаковать
        if (distance <= attackRange)
        {
            decimal damage = actor.Stats.Get(StatType.Attack);

            var attackAction = new AttackAction(actor, world.Player, damage);
            return attackAction.Execute();
        }

        // Иначе пробуем двигаться в сторону персонажа игрока
        List<Position>? path = _pathFinder.FindPath(world, actor.Position, world.Player.Position);

        if (path is null || path.Count == 0)
        {
            return [new IdleLogEntry(actor.Id)];
        }

        var trimmed = path.Skip(1).ToList();
        if (!trimmed.Any())
        {
            return [new IdleLogEntry(actor.Id)];
        }

        int moveRange = (int)Math.Floor(actor.Stats.Get(StatType.MoveRange));
        int stepsToMove = Math.Min(moveRange, trimmed.Count);
        Position target = trimmed[stepsToMove - 1];

        var moveAction = new MoveAction(actor, target);
        return moveAction.Execute();
    }
}
