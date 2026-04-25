using Domain.ValueObjects;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Units;

namespace GameRuntime.Common;

public static class MovementRules
{
    public static bool CanStandOn(ArenaWorld world, BaseUnit actor, Position position)
    {
        if (!position.IsWithinGrid(world.Arena.GridWidth, world.Arena.GridHeight))
        {
            return false;
        }

        if (world.Arena.BlockedPositions.Contains(position))
        {
            return false;
        }

        if (world.Player != actor && !world.Player.IsDead && world.Player.Position == position)
        {
            return false;
        }

        if (world.Enemies.Any(e => e != actor && !e.IsDead && e.Position == position))
        {
            return false;
        }

        return true;
    }

    public static Position? SelectMoveTarget(
        ArenaWorld world,
        BaseUnit actor,
        IReadOnlyList<Position> path,
        int moveRange)
    {
        if (path.Count < 2 || moveRange <= 0)
        {
            return null;
        }

        Position? target = null;

        foreach (Position step in path.Skip(1).Take(moveRange))
        {
            if (!CanStandOn(world, actor, step))
            {
                break;
            }

            target = step;
        }

        return target;
    }
}
