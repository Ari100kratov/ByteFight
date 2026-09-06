using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.ValueObjects;
using GameRuntime.Common;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Abilities;
using GameRuntime.Common.World.Units;
using GameRuntime.Logic.Actions;
using GameRuntime.Logic.NPC.PathFinding;
using GameRuntime.Logic.User.Api;

namespace GameRuntime.Logic.User.Execution;

/// <summary>
/// Исполняет действия, возвращённые пользовательским скриптом,
/// и преобразует их в игровые события.
///
/// Отвечает за проверку корректности действия, применение игровых правил
/// и делегирование фактического перемещения или атаки игровым action-классам.
/// </summary>
internal sealed class UserActionExecutor(IPathFinder pathFinder)
{
    private readonly IPathFinder _pathFinder = pathFinder;

    /// <summary>
    /// Выполняет пользовательское действие от имени указанного юнита
    /// в контексте текущего мира.
    /// </summary>
    /// <param name="action">Действие, возвращённое пользовательским кодом.</param>
    /// <param name="actor">Юнит, совершающий действие.</param>
    /// <param name="world">Текущее состояние игрового мира.</param>
    public IEnumerable<GameActionLogEntry> Execute(
        UserAction action,
        BaseUnit actor,
        ArenaWorld world)
    {
        return action switch
        {
            Attack a => ExecuteAttack(a, actor, world),
            MoveTo m => ExecuteMoveTo(m, actor, world),
            MoveTowards m => ExecuteMoveTowards(m, actor, world),
            MoveTowardsPosition m => ExecuteMoveTowardsPosition(m, actor, world),
            MoveAwayFrom m => ExecuteMoveAwayFrom(m, actor, world),
            Idle => [world.CreateIdleLogEntry(actor, IdleReasons.ManualIdle)],
            _ => [world.CreateIdleLogEntry(actor, IdleReasons.InvalidAction)]
        };
    }

    /// <summary>
    /// Выполняет атаку по указанной цели, если она существует,
    /// жива и находится в пределах дальности атаки.
    /// </summary>
    private IEnumerable<GameActionLogEntry> ExecuteAttack(
        Attack action,
        BaseUnit actor,
        ArenaWorld world)
    {
        BaseUnit target = world.GetUnit(action.TargetId);

        if (target.IsDead)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.TargetDead)];
        }

        int distance = actor.Position.HexDistance(target.Position);

        RuntimeAbility? attack = actor.Abilities.FindBestBasicAttack(distance);

        if (attack is null)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.OutOfRange)];
        }

        return new UseAbilityAction(actor, attack, target.Position).Execute(world);
    }

    /// <summary>
    /// Выполняет перемещение к указанной позиции.
    ///
    /// Движок строит путь до цели, а затем проходит по нему столько гексов,
    /// сколько позволяет остаток очков перемещения.
    /// Если путь отсутствует или перемещение невозможно, действие заменяется на Idle.
    /// </summary>
    private IEnumerable<GameActionLogEntry> ExecuteMoveTo(
        MoveTo action,
        BaseUnit actor,
        ArenaWorld world)
    {
        List<Position>? path = _pathFinder.FindPath(world, actor.Position, action.Target);

        return ExecutePath(path, actor, world);
    }

    /// <summary>
    /// Выполняет сближение с указанной целью.
    ///
    /// В отличие от <see cref="ExecuteMoveTo" />, действие не требует попасть именно
    /// в клетку цели. Если цель занята или полностью окружена, исполнитель
    /// всё равно попытается выбрать полезный путь в её сторону.
    /// </summary>
    private IEnumerable<GameActionLogEntry> ExecuteMoveTowards(
        MoveTowards action,
        BaseUnit actor,
        ArenaWorld world)
    {
        BaseUnit target = world.GetUnit(action.TargetId);

        if (target.IsDead)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.TargetDead)];
        }

        List<Position>? path = _pathFinder.FindPathTowards(
            world,
            actor.Position,
            target.Position);

        return ExecutePath(path, actor, world);
    }

    /// <summary>
    /// Выполняет движение в сторону указанной позиции.
    /// </summary>
    private IEnumerable<GameActionLogEntry> ExecuteMoveTowardsPosition(
        MoveTowardsPosition action,
        BaseUnit actor,
        ArenaWorld world)
    {
        List<Position>? path = _pathFinder.FindPathTowards(
            world,
            actor.Position,
            action.Target);

        return ExecutePath(path, actor, world);
    }

    /// <summary>
    /// Выполняет отступление от указанной цели.
    ///
    /// Из всех достижимых за текущий ход гексов выбирается тот,
    /// который максимизирует расстояние до цели.
    /// Если допустимого гекса нет, действие заменяется на Idle.
    /// </summary>
    private IEnumerable<GameActionLogEntry> ExecuteMoveAwayFrom(
        MoveAwayFrom action,
        BaseUnit actor,
        ArenaWorld world)
    {
        BaseUnit target = world.GetUnit(action.TargetId);

        if (target.IsDead)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.TargetDead)];
        }

        System.Collections.Immutable.ImmutableHashSet<Position> reachable =
            MovementRules.GetReachableCells(world, actor);

        Position? bestPosition = reachable
            .Where(p => p != actor.Position)
            .OrderByDescending(p => p.HexDistance(target.Position))
            .ThenBy(p => p.HexDistance(actor.Position))
            .Cast<Position?>()
            .FirstOrDefault();

        if (bestPosition is null)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.NoPath)];
        }

        List<Position>? path = _pathFinder.FindPath(world, actor.Position, bestPosition);

        return ExecutePath(path, actor, world);
    }

    /// <summary>
    /// Проходит по построенному пути в пределах оставшихся очков перемещения.
    /// </summary>
    private IEnumerable<GameActionLogEntry> ExecutePath(
        List<Position>? path,
        BaseUnit actor,
        ArenaWorld world)
    {
        if (path is null || path.Count < 2)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.NoPath)];
        }

        List<Position> trimmed = TrimPathToBudget(world, actor, path);

        if (trimmed.Count < 2)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.MoveImpossible)];
        }

        return new MoveAction(actor, trimmed).Execute(world);
    }

    private static List<Position> TrimPathToBudget(
        ArenaWorld world,
        BaseUnit actor,
        List<Position> path)
    {
        int budget = actor.BattleState.MovePointsRemaining;
        int spent = 0;
        int last = 0;

        for (int i = 1; i < path.Count; i++)
        {
            int cost = MovementRules.MovementCost(world, path[i]);

            if (cost == int.MaxValue || spent + cost > budget)
            {
                break;
            }

            spent += cost;
            last = i;
        }

        return [.. path.Take(last + 1)];
    }
}
