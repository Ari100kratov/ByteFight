using Domain.ValueObjects;
using GameRuntime.Common.World;

namespace GameRuntime.Logic.NPC.PathFinding;

internal interface IPathFinder
{
    List<Position>? FindPath(ArenaWorld world, Position start, Position target);
}
