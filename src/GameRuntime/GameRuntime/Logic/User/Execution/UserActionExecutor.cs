using System.Collections.Immutable;
using Domain.Game.Stats;
using Domain.GameRuntime.GameActionLogs;
using Domain.ValueObjects;
using GameRuntime.Common;
using GameRuntime.Common.World;
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
internal sealed class UserActionExecutor
{
    private readonly IPathFinder _pathFinder;

    public UserActionExecutor(IPathFinder pathFinder)
    {
        _pathFinder = pathFinder;
    }

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

        int distance = actor.Position.ManhattanDistance(target.Position);
        int attackRange = (int)Math.Ceiling(actor.Stats.Get(StatType.AttackRange));

        if (distance > attackRange)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.OutOfRange)];
        }

        decimal damage = actor.Stats.Get(StatType.Attack);
        return new AttackAction(actor, target, damage).Execute(world);
    }

    /// <summary>
    /// Выполняет перемещение к указанной позиции.
    ///
    /// Движок строит путь до цели, а затем проходит по нему столько клеток,
    /// сколько позволяет характеристика <c>MoveRange</c>.
    /// Если путь отсутствует или перемещение невозможно, действие заменяется на Idle.
    /// </summary>
    private IEnumerable<GameActionLogEntry> ExecuteMoveTo(
        MoveTo action,
        BaseUnit actor,
        ArenaWorld world)
    {
        List<Position>? path = _pathFinder.FindPath(
        world,
        actor.Position,
        action.Target);

        if (path is null || path.Count < 2)
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

        return new MoveAction(actor, target).Execute(world);
    }

    /// <summary>
    /// Выполняет сближение с указанной целью.
    ///
    /// Это высокоуровневое действие: пользователь указывает только цель,
    /// а фактическое движение реализуется через обычный <see cref="MoveTo"/>,
    /// используя текущую позицию цели как точку назначения.
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

        return ExecuteMoveTo(new MoveTo(target.Position), actor, world);
    }

    /// <summary>
    /// Выполняет отступление от указанной цели.
    ///
    /// Из всех достижимых за текущий ход клеток выбирается та,
    /// которая максимизирует расстояние до цели.
    /// Если допустимой клетки нет, действие заменяется на Idle.
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

        int moveRange = (int)Math.Floor(actor.Stats.Get(StatType.MoveRange));
        ImmutableHashSet<Position> reachable = GetReachableCells(world, actor, actor.Position, moveRange);

        Position? bestPosition = reachable
            .Where(p => p != actor.Position)
            .OrderByDescending(p => p.ManhattanDistance(target.Position))
            .ThenBy(p => p.ManhattanDistance(actor.Position))
            .Cast<Position?>()
            .FirstOrDefault();

        if (bestPosition is null)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.NoPath)];
        }

        return new MoveAction(actor, bestPosition).Execute(world);
    }

    /// <summary>
    /// Возвращает все клетки, достижимые из стартовой позиции
    /// за указанное количество шагов.
    ///
    /// Используется для логики отступления и может быть полезен
    /// как основа для будущих более сложных алгоритмов выбора позиции.
    /// </summary>
    private ImmutableHashSet<Position> GetReachableCells(
        ArenaWorld world,
        BaseUnit actor,
        Position start,
        int maxDistance)
    {
        var result = new HashSet<Position> { start };
        var queue = new Queue<(Position Position, int Distance)>();

        queue.Enqueue((start, 0));

        while (queue.Count > 0)
        {
            (Position current, int distance) = queue.Dequeue();

            if (distance >= maxDistance)
            {
                continue;
            }

            foreach (Position neighbor in GetNeighbors(current, world.Arena))
            {
                if (result.Contains(neighbor))
                {
                    continue;
                }

                if (!MovementRules.CanStandOn(world, actor, neighbor))
                {
                    continue;
                }

                result.Add(neighbor);
                queue.Enqueue((neighbor, distance + 1));
            }
        }

        return [.. result];
    }

    /// <summary>
    /// Возвращает ортогональных соседей для указанной позиции
    /// в пределах арены.
    /// </summary>
    private static IEnumerable<Position> GetNeighbors(Position position, ArenaDefinition arena)
    {
        if (position.X + 1 < arena.GridWidth)
        {
            yield return new Position(position.X + 1, position.Y);
        }

        if (position.X - 1 >= 0)
        {
            yield return new Position(position.X - 1, position.Y);
        }

        if (position.Y + 1 < arena.GridHeight)
        {
            yield return new Position(position.X, position.Y + 1);
        }

        if (position.Y - 1 >= 0)
        {
            yield return new Position(position.X, position.Y - 1);
        }
    }
}
