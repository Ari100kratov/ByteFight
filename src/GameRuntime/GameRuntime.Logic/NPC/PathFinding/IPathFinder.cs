using Domain.ValueObjects;
using GameRuntime.Core;

namespace GameRuntime.Logic.NPC.PathFinding;

public interface IPathFinder
{
    List<Position>? FindPath(ArenaWorld world, Position start, Position goal);
}
