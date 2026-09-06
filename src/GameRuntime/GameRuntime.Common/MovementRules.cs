using System.Collections.Immutable;
using Domain.Game.Arenas;
using Domain.ValueObjects;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Units;

namespace GameRuntime.Common;

/// <summary>
/// Общие правила перемещения юнитов по гексагональному полю арены:
/// проходимость, стоимость входа и трата очков перемещения.
/// </summary>
public static class MovementRules
{
    /// <summary>
    /// Проверяет, может ли юнит находиться на указанном гексе.
    /// </summary>
    public static bool CanStandOn(ArenaWorld world, BaseUnit actor, Position position)
    {
        if (!world.Arena.IsWithin(position))
        {
            return false;
        }

        if (world.Arena.BlockedPositions.Contains(position))
        {
            return false;
        }

        if (!TerrainRules.IsPassable(world.Arena.TerrainAt(position)))
        {
            return false;
        }

        return !world.IsOccupiedByOther(position, actor);
    }

    /// <summary>
    /// Стоимость входа на гекс в очках перемещения.
    /// Непроходимые гексы имеют бесконечную стоимость.
    /// </summary>
    public static int MovementCost(ArenaWorld world, Position position)
    {
        if (world.Arena.BlockedPositions.Contains(position))
        {
            return int.MaxValue;
        }

        return TerrainRules.MovementCost(world.Arena.TerrainAt(position));
    }

    /// <summary>
    /// Выполняет поиск Дейкстры от позиции юнита в пределах его очков
    /// перемещения. Возвращает достижимые гексы и стоимость входа.
    /// Стартовая клетка включена с нулевой стоимостью.
    /// </summary>
    public static ImmutableDictionary<Position, int> GetReachableCellCosts(
        ArenaWorld world,
        BaseUnit actor,
        int movePoints)
    {
        var costs = new Dictionary<Position, int> { [actor.Position] = 0 };
        var queue = new PriorityQueue<Position, int>();
        queue.Enqueue(actor.Position, 0);

        while (queue.TryDequeue(out Position current, out int currentCost))
        {
            if (costs.GetValueOrDefault(current, int.MaxValue) < currentCost)
            {
                continue;
            }

            foreach (Position neighbor in current.GetHexNeighbors())
            {
                if (!CanStandOn(world, actor, neighbor))
                {
                    continue;
                }

                int stepCost = MovementCost(world, neighbor);

                if (stepCost == int.MaxValue)
                {
                    continue;
                }

                int newCost = currentCost + stepCost;

                if (newCost > movePoints)
                {
                    continue;
                }

                if (newCost < costs.GetValueOrDefault(neighbor, int.MaxValue))
                {
                    costs[neighbor] = newCost;
                    queue.Enqueue(neighbor, newCost);
                }
            }
        }

        return costs.ToImmutableDictionary();
    }

    /// <summary>
    /// Возвращает множество гексов, достижимых юнитом за текущий ход
    /// с учётом рельефа, препятствий и занятых клеток.
    /// </summary>
    public static ImmutableHashSet<Position> GetReachableCells(ArenaWorld world, BaseUnit actor)
    {
        return GetReachableCellCosts(world, actor, actor.BattleState.MovePointsRemaining)
            .Keys
            .Where(p => p != actor.Position)
            .ToImmutableHashSet();
    }

    /// <summary>
    /// Проверяет путь юнита: шаги должны быть по соседним гексам,
    /// суммарная стоимость — не больше оставшихся очков перемещения,
    /// каждый промежуточный гекс — проходим и свободен.
    /// </summary>
    public static bool IsPathValid(
        ArenaWorld world,
        BaseUnit actor,
        IReadOnlyList<Position> path,
        out int totalCost)
    {
        totalCost = 0;

        if (path.Count < 2 || path[0] != actor.Position)
        {
            return false;
        }

        for (int i = 1; i < path.Count; i++)
        {
            Position previous = path[i - 1];
            Position step = path[i];

            if (previous.HexDistance(step) != 1)
            {
                return false;
            }

            if (!CanStandOn(world, actor, step))
            {
                return false;
            }

            int cost = MovementCost(world, step);

            if (cost == int.MaxValue)
            {
                return false;
            }

            totalCost += cost;
        }

        return totalCost <= actor.BattleState.MovePointsRemaining;
    }
}
