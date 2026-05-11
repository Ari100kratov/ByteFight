using Domain.ValueObjects;
using GameRuntime.Common.World;

namespace GameRuntime.Logic.NPC.PathFinding;

internal interface IPathFinder
{
    /// <summary>
    /// Ищет строгий путь от стартовой клетки до целевой клетки.
    ///
    /// Целевая клетка считается допустимой точкой маршрута, даже если она занята
    /// другим живым юнитом. Это нужно, чтобы можно было построить путь к позиции цели,
    /// а затем выбрать ближайшую допустимую клетку для остановки.
    ///
    /// Если путь до указанной клетки отсутствует, возвращает <see langword="null" />.
    /// </summary>
    /// <param name="world">Текущее состояние игрового мира.</param>
    /// <param name="start">Стартовая позиция.</param>
    /// <param name="target">Целевая позиция.</param>
    /// <returns>Список клеток пути, включая стартовую и целевую, либо <see langword="null" />.</returns>
    List<Position>? FindPath(ArenaWorld world, Position start, Position target);

    /// <summary>
    /// Ищет путь в сторону целевой клетки.
    ///
    /// Сначала пытается построить строгий путь через <see cref="FindPath" />.
    /// Если это невозможно, выбирает лучшую достижимую клетку, которая максимально
    /// приближает юнита к цели.
    ///
    /// Используется для высокоуровневого поведения вроде "двигаться к юниту",
    /// когда сама цель может быть занята или полностью окружена.
    /// </summary>
    /// <param name="world">Текущее состояние игрового мира.</param>
    /// <param name="start">Стартовая позиция.</param>
    /// <param name="target">Позиция цели, в сторону которой нужно двигаться.</param>
    /// <returns>Путь к целевой или ближайшей полезной клетке, либо <see langword="null" />.</returns>
    List<Position>? FindPathTowards(ArenaWorld world, Position start, Position target);
}
