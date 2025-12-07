using Domain.ValueObjects;
using GameRuntime.World;

namespace GameRuntime.Logic.NPC.PathFinding;

internal interface IPathFinder
{
    List<Position>? FindPath(ArenaWorld world, Position start, Position goal);
}
