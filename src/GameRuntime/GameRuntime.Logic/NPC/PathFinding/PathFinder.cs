using Domain.ValueObjects;
using GameRuntime.Core;
using GameRuntime.Core.Units;

namespace GameRuntime.Logic.NPC.PathFinding;

public sealed class PathFinder : IPathFinder
{
    public List<Position>? FindPath(ArenaWorld world, Position start, Position goal)
    {
        ArenaDefinition arena = world.Arena;

        if (!IsWithin(arena, start) || !IsWithin(arena, goal))
        {
            return null;
        }

        var occupied = new HashSet<Position>
        {
            world.Player.Position
        };

        foreach (EnemyUnit enemy in world.Enemies)
        {
            if (enemy.Position != start)
            {
                occupied.Add(enemy.Position);
            }
        }

        var open = new PriorityQueue<Position, int>();
        var cameFrom = new Dictionary<Position, Position>();
        var gScore = new Dictionary<Position, int>();
        var fScore = new Dictionary<Position, int>();

        gScore[start] = 0;
        fScore[start] = Heuristic(start, goal);
        open.Enqueue(start, fScore[start]);

        var visited = new HashSet<Position>();

        while (open.Count > 0)
        {
            open.TryDequeue(out Position current, out _);

            if (current!.Equals(goal))
            {
                return ReconstructPath(cameFrom, current);
            }

            visited.Add(current);

            foreach (Position neigh in GetNeighbors(current))
            {
                if (!IsWithin(arena, neigh))
                {
                    continue;
                }

                if (occupied.Contains(neigh) && !neigh.Equals(goal))
                {
                    continue;
                }

                if (visited.Contains(neigh))
                {
                    continue;
                }

                int tentativeG = gScore[current] + 1;

                if (!gScore.TryGetValue(neigh, out int gExisting) || tentativeG < gExisting)
                {
                    cameFrom[neigh] = current;
                    gScore[neigh] = tentativeG;
                    fScore[neigh] = tentativeG + Heuristic(neigh, goal);
                    open.Enqueue(neigh, fScore[neigh]);
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

    private static int Heuristic(Position positionA, Position positionB) =>
        positionA.ManhattanDistance(positionB);

    private static IEnumerable<Position> GetNeighbors(Position p)
    {
        yield return new Position(p.X + 1, p.Y);

        if (p.X - 1 >= 0)
        {
            yield return new Position(p.X - 1, p.Y);
        }

        yield return new Position(p.X, p.Y + 1);

        if (p.Y - 1 >= 0)
        {
            yield return new Position(p.X, p.Y - 1);
        }
    }

    private static bool IsWithin(ArenaDefinition arena, Position position) =>
        position.IsWithinGrid(arena.GridWidth, arena.GridHeight);
}
