using Domain.Game.Stats;
using Domain.GameRuntime.GameActionLogs;
using Domain.ValueObjects;
using GameRuntime.Logic.Actions;
using GameRuntime.Logic.NPC.PathFinding;
using GameRuntime.Logic.User.Api;
using GameRuntime.World;
using GameRuntime.World.Units;

namespace GameRuntime.Logic.User.Execution;

internal sealed class UserActionExecutor
{
    private readonly IPathFinder _pathFinder;

    public UserActionExecutor(IPathFinder pathFinder)
    {
        _pathFinder = pathFinder;
    }

    public IEnumerable<GameActionLogEntry> Execute(
        UserAction action,
        BaseUnit actor,
        ArenaWorld world)
    {
        return action switch
        {
            Attack a => ExecuteAttack(a, actor, world),
            MoveTo m => ExecuteMoveTo(m, actor, world),
            Idle => [world.CreateIdleLogEntry(actor.Id, IdleReasons.ManualIdle)],
            _ => [world.CreateIdleLogEntry(actor.Id, IdleReasons.InvalidAction)]
        };
    }

    private IEnumerable<GameActionLogEntry> ExecuteAttack(
        Attack action,
        BaseUnit actor,
        ArenaWorld world)
    {
        BaseUnit target = world.GetUnit(action.TargetId);
        if (target.IsDead)
        {
            return [world.CreateIdleLogEntry(actor.Id, IdleReasons.TargetDead)];
        }

        int distance = actor.Position.ManhattanDistance(target.Position);
        int attackRange = (int)Math.Ceiling(actor.Stats.Get(StatType.AttackRange));

        if (distance > attackRange)
        {
            return [world.CreateIdleLogEntry(actor.Id, IdleReasons.OutOfRange)];
        }

        decimal damage = actor.Stats.Get(StatType.Attack);
        return new AttackAction(actor, target, damage).Execute(world);
    }

    private IEnumerable<GameActionLogEntry> ExecuteMoveTo(MoveTo action, BaseUnit actor, ArenaWorld world)
    {
        // 1. Находим путь
        List<Position>? path = _pathFinder.FindPath(
            world,
            actor.Position,
            action.Target
        );

        if (path is null || path.Count < 2)
        {
            return [world.CreateIdleLogEntry(actor.Id, IdleReasons.NoPath)];
        }

        // 2. Ограничиваем дальность
        int moveRange = (int)Math.Floor(actor.Stats.Get(StatType.MoveRange));

        Position target = path
            .Skip(1)
            .Take(moveRange)
            .Last();

        // 3. Выполняем
        return new MoveAction(actor, target).Execute(world);
    }
}
