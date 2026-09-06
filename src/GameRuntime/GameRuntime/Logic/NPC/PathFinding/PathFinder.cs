using Domain.ValueObjects;
using GameRuntime.Common;
using GameRuntime.Common.World;

namespace GameRuntime.Logic.NPC.PathFinding;

/// <summary>
/// A* по гексагональной сетке со стоимостью рельефа.
/// </summary>
internal sealed class PathFinder : IPathFinder
{
    /// <inheritdoc />
    public List<Position>? FindPath(ArenaWorld world, Position start, Position target)
    {
        ArenaDefinition arena = world.Arena;

        if (!arena.IsWithin(start) || !arena.IsWithin(target))
        {
            return null;
        }

        var open = new PriorityQueue<Position, int>();
        var cameFrom = new Dictionary<Position, Position>();
        var gScore = new Dictionary<Position, int> { [start] = 0 };

        open.Enqueue(start, start.HexDistance(target));

        while (open.TryDequeue(out Position current, out _))
        {
            if (current == target)
            {
                return ReconstructPath(cameFrom, current);
            }

            foreach (Position neighbor in current.GetHexNeighbors())
            {
                // Занятые гексы непроходимы, включая гекс цели:
                // путь строится до соседней с ней клетки.
                if (!arena.IsWithin(neighbor) || world.GetOccupant(neighbor) is not null)
                {
                    continue;
                }

                int stepCost = MovementRules.MovementCost(world, neighbor);

                if (stepCost == int.MaxValue)
                {
                    continue;
                }

                int tentativeG = gScore[current] + stepCost;

                if (tentativeG < gScore.GetValueOrDefault(neighbor, int.MaxValue))
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    open.Enqueue(neighbor, tentativeG + neighbor.HexDistance(target));
                }
            }
        }

        return null;
    }

    /// <inheritdoc />
    public List<Position>? FindPathTowards(ArenaWorld world, Position start, Position target)
    {
        // Ищем полный путь; если цель недостижима (занята/окружена),
        // строим путь до ближайшего к ней достижимого гекса.
        List<Position>? direct = FindPath(world, start, target);

        if (direct is not null)
        {
            return direct;
        }

        var cameFrom = new Dictionary<Position, Position>();
        var gScore = new Dictionary<Position, int> { [start] = 0 };
        var open = new PriorityQueue<Position, int>();

        open.Enqueue(start, start.HexDistance(target));

        Position? best = null;
        int bestDistance = start.HexDistance(target);

        while (open.TryDequeue(out Position current, out _))
        {
            int distance = current.HexDistance(target);

            if (current != start && distance < bestDistance)
            {
                best = current;
                bestDistance = distance;
            }

            foreach (Position neighbor in current.GetHexNeighbors())
            {
                if (!world.Arena.IsWithin(neighbor) || world.GetOccupant(neighbor) is not null)
                {
                    continue;
                }

                int stepCost = MovementRules.MovementCost(world, neighbor);

                if (stepCost == int.MaxValue)
                {
                    continue;
                }

                int tentativeG = gScore[current] + stepCost;

                if (tentativeG < gScore.GetValueOrDefault(neighbor, int.MaxValue))
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    open.Enqueue(neighbor, tentativeG + neighbor.HexDistance(target));
                }
            }
        }

        return best is null ? null : ReconstructPath(cameFrom, best);
    }

    private static List<Position> ReconstructPath(
        Dictionary<Position, Position> cameFrom,
        Position current)
    {
        var path = new List<Position> { current };

        while (cameFrom.TryGetValue(current, out Position previous))
        {
            current = previous;
            path.Add(current);
        }

        path.Reverse();

        return path;
    }
}
