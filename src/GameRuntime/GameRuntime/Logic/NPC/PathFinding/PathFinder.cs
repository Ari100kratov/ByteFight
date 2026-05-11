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

        var open = new PriorityQueue<Position, PathPriority>();
        var cameFrom = new Dictionary<Position, Position>();
        var gScore = new Dictionary<Position, int>();
        var visited = new HashSet<Position>();

        gScore[start] = 0;
        open.Enqueue(start, CreatePriority(start, target, g: 0));

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

                    open.Enqueue(neigh, CreatePriority(neigh, target, tentativeG));
                }
            }
        }

        return null;
    }

    public List<Position>? FindPathTowards(ArenaWorld world, Position start, Position target)
    {
        ArenaDefinition arena = world.Arena;

        if (!IsWithin(arena, start) || !IsWithin(arena, target))
        {
            return null;
        }

        HashSet<Position> occupied = CreateOccupiedPositions(world, start);

        var open = new PriorityQueue<Position, PathPriority>();
        var cameFrom = new Dictionary<Position, Position>();
        var gScore = new Dictionary<Position, int>();
        var visited = new HashSet<Position>();

        Position? bestPosition = null;
        int startDistance = start.ManhattanDistance(target);

        gScore[start] = 0;
        open.Enqueue(start, CreatePriority(start, target, g: 0));

        while (open.TryDequeue(out Position current, out _))
        {
            if (!visited.Add(current))
            {
                continue;
            }

            if (current != start &&
                current.ManhattanDistance(target) < startDistance &&
                IsBetterTowardsTarget(current, bestPosition, target, gScore))
            {
                bestPosition = current;
            }

            foreach (Position neighbor in GetNeighbors(current, arena))
            {
                if (visited.Contains(neighbor))
                {
                    continue;
                }

                if (arena.BlockedPositions.Contains(neighbor) || occupied.Contains(neighbor))
                {
                    continue;
                }

                int tentativeG = gScore[current] + 1;

                if (!gScore.TryGetValue(neighbor, out int existingG) || tentativeG < existingG)
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;

                    open.Enqueue(neighbor, CreatePriority(neighbor, target, tentativeG));
                }
            }
        }

        return bestPosition is null ? null : ReconstructPath(cameFrom, bestPosition);
    }

    private static bool IsBetterTowardsTarget(
        Position candidate,
        Position? currentBest,
        Position target,
        Dictionary<Position, int> gScore)
    {
        if (currentBest is null)
        {
            return true;
        }

        int candidateDistance = candidate.ManhattanDistance(target);
        int bestDistance = currentBest.ManhattanDistance(target);

        if (candidateDistance != bestDistance)
        {
            return candidateDistance < bestDistance;
        }

        return gScore[candidate] < gScore[currentBest];
    }

    private static HashSet<Position> CreateOccupiedPositions(ArenaWorld world, Position start)
    {
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

        return occupied;
    }

    private readonly record struct PathPriority(
        int TotalCost,
        int DistanceSquared) : IComparable<PathPriority>
    {
        public int CompareTo(PathPriority other)
        {
            int costComparison = TotalCost.CompareTo(other.TotalCost);

            return costComparison != 0
                ? costComparison
                : DistanceSquared.CompareTo(other.DistanceSquared);
        }
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

    private static int DistanceToTargetSquared(Position from, Position to)
    {
        int dx = from.X - to.X;
        int dy = from.Y - to.Y;

        return dx * dx + dy * dy;
    }

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

    private static PathPriority CreatePriority(Position position, Position target, int g)
    {
        int h = Heuristic(position, target);

        return new PathPriority(
            TotalCost: g + h,
            DistanceSquared: DistanceToTargetSquared(position, target));
    }
}
