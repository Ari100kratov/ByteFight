using Domain.ValueObjects;
using GameRuntime.Common.World;

namespace GameRuntime.Logic.NPC.PathFinding;

internal interface IPathFinder
{
    /// <summary>
    /// Ищет путь от стартовой клетки до целевой клетки с учетом границ арены,
    /// заблокированных клеток и занятых клеток другими живыми юнитами.
    /// 
    /// Если несколько путей имеют одинаковую стоимость, выбирается вариант,
    /// который визуально сильнее приближает юнита к цели.
    /// </summary>
    List<Position>? FindPath(ArenaWorld world, Position start, Position target);
}
