using Domain.ValueObjects;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Units;

namespace GameRuntime.Logic.NPC.PathFinding;

internal sealed class PathFinder : IPathFinder
{
    public List<Position>? FindPath(ArenaWorld world, Position start, Position target)
    {
        ArenaDefinition arena = world.Arena;

        if (!IsWithin(arena, start) || !IsWithin(arena, target))
        {
            return null;
        }

        var occupied = new HashSet<Position>();

        if (!world.Player.IsDead && world.Player.Position != start)
        {
            occupied.Add(world.Player.Position);
        }

        foreach (EnemyUnit enemy in world.Enemies)
        {
            if (!enemy.IsDead && enemy.Position != start)
            {
                occupied.Add(enemy.Position);
            }
        }

        var open = new PriorityQueue<Position, int>();
        var cameFrom = new Dictionary<Position, Position>();
        var gScore = new Dictionary<Position, int>();
        var visited = new HashSet<Position>();

        gScore[start] = 0;
        open.Enqueue(start, Heuristic(start, target));

        while (open.TryDequeue(out Position current, out _))
        {
            if (current.Equals(target))
            {
                return ReconstructPath(cameFrom, current);
            }

            if (!visited.Add(current))
            {
                continue;
            }

            foreach (Position neigh in GetNeighbors(current, arena))
            {
                if (!IsWithin(arena, neigh))
                {
                    continue;
                }

                if (visited.Contains(neigh))
                {
                    continue;
                }

                bool isBlocked = arena.BlockedPositions.Contains(neigh);
                bool isOccupied = occupied.Contains(neigh);

                if ((isBlocked || isOccupied) && !neigh.Equals(target))
                {
                    continue;
                }

                int tentativeG = gScore[current] + 1;

                if (!gScore.TryGetValue(neigh, out int existingG) || tentativeG < existingG)
                {
                    cameFrom[neigh] = current;
                    gScore[neigh] = tentativeG;

                    int priority = tentativeG + Heuristic(neigh, target);
                    open.Enqueue(neigh, priority);
                }
            }
        }

        return null;
    }

    private static List<Position> ReconstructPath(Dictionary<Position, Position> cameFrom, Position current)
    {
        var total = new List<Position> { current };

        while (cameFrom.TryGetValue(current, out Position prev))
        {
            current = prev;
            total.Add(current);
        }

        total.Reverse();
        return total;
    }

    private static int Heuristic(Position from, Position to) =>
        Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);

    private static IEnumerable<Position> GetNeighbors(Position p, ArenaDefinition arena)
    {
        if (p.X + 1 < arena.GridWidth)
        {
            yield return new Position(p.X + 1, p.Y);
        }

        if (p.X - 1 >= 0)
        {
            yield return new Position(p.X - 1, p.Y);
        }

        if (p.Y + 1 < arena.GridHeight)
        {
            yield return new Position(p.X, p.Y + 1);
        }

        if (p.Y - 1 >= 0)
        {
            yield return new Position(p.X, p.Y - 1);
        }
    }

    private static bool IsWithin(ArenaDefinition arena, Position position) =>
        position.IsWithinGrid(arena.GridWidth, arena.GridHeight);
}
