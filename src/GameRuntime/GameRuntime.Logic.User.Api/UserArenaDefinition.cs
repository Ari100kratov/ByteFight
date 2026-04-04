using Domain.ValueObjects;

namespace GameRuntime.Logic.User.Api;

/// <summary>
/// Описание игровой арены
/// </summary>
public sealed record UserArenaDefinition
{
    /// <summary>
    /// Ширина игровой сетки в клетках по оси X.
    /// </summary>
    public required int GridWidth { get; init; }

    /// <summary>
    /// Высота игровой сетки в клетках по оси Y.
    /// </summary>
    public required int GridHeight { get; init; }

    /// <summary>
    /// Начальная позиция управляемого игроком юнита на арене.
    /// </summary>
    public required Position StartPosition { get; init; }

    /// <summary>
    /// Список клеток арены, занятых статическими препятствиями
    /// и недоступных для перемещения.
    /// </summary>
    public required IReadOnlyList<Position> BlockedPositions { get; init; }

    /// <summary>
    /// Проверяет, находится ли позиция внутри границ арены.
    /// </summary>
    public bool IsWithin(Position position)
        => position.IsWithinGrid(GridWidth, GridHeight);

    /// <summary>
    /// Проверяет, является ли клетка заблокированной.
    /// </summary>
    public bool IsBlocked(Position position)
        => BlockedPositions.Contains(position);

    /// <summary>
    /// Возвращает соседние клетки по ортогонали, которые находятся
    /// внутри границ арены.
    ///
    /// Метод не исключает заблокированные клетки и не учитывает занятость юнитами.
    /// Для этого нужно дополнительно использовать проверки мира.
    /// </summary>
    /// <param name="position">Позиция, для которой нужно получить соседей.</param>
    public IEnumerable<Position> GetNeighbors4(Position position)
        => position.GetNeighbors4().Where(IsWithin);

    /// <summary>
    /// Возвращает соседние клетки по ортогонали и диагонали,
    /// которые находятся внутри границ арены.
    ///
    /// Метод не исключает препятствия и не учитывает занятость юнитами.
    /// </summary>
    /// <param name="position">Позиция, для которой нужно получить соседей.</param>
    public IEnumerable<Position> GetNeighbors8(Position position)
        => position.GetNeighbors8().Where(IsWithin);
}
