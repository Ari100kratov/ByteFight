using Domain.GameRuntime.GameActionLogs;

namespace Domain.ValueObjects;

/// <summary>
/// Координата точки на двумерной сетке арены.
/// </summary>
public sealed record Position
{
    /// <summary>
    /// Координата по оси X.
    /// </summary>
    public int X { get; init; }

    /// <summary>
    /// Координата по оси Y.
    /// </summary>
    public int Y { get; init; }

    private Position() { } // Необходимо для EF

    /// <summary>
    /// Создаёт позицию на сетке.
    /// </summary>
    /// <param name="x">Координата X (неотрицательная).</param>
    /// <param name="y">Координата Y (неотрицательная).</param>
    public Position(int x, int y)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(x);
        ArgumentOutOfRangeException.ThrowIfNegative(y);

        X = x;
        Y = y;
    }

    /// <summary>
    /// Проверяет, находится ли точка в границах сетки.
    /// </summary>
    public bool IsWithinGrid(int gridWidth, int gridHeight)
        => X >= 0 && X < gridWidth && Y >= 0 && Y < gridHeight;

    /// <summary>
    /// Манхэттенское расстояние до другой позиции.
    /// </summary>
    public int ManhattanDistance(Position position) => Math.Abs(X - position.X) + Math.Abs(Y - position.Y);

    /// <summary>
    /// Евклидово расстояние до другой позиции.
    /// </summary>
    public double EuclideanDistance(Position position) =>
        Math.Sqrt((X - position.X) * (X - position.X) + (Y - position.Y) * (Y - position.Y));

    /// <summary>
    /// Вычисляет направление взгляда в сторону целевой позиции.
    /// </summary>
    public FacingDirection CalculateFacing(Position targetPosition) =>
        targetPosition.X < X ? FacingDirection.Left : FacingDirection.Right;

    /// <summary>
    /// Возвращает четыре соседние клетки по ортогонали:
    /// вправо, влево, вниз и вверх.
    ///
    /// Метод не проверяет, находятся ли возвращаемые клетки
    /// внутри границ конкретной арены, поэтому результат при необходимости
    /// нужно дополнительно фильтровать через <c>UserArenaDefinition.IsWithin</c>
    /// или аналогичную проверку.
    /// </summary>
    public IEnumerable<Position> GetNeighbors4()
    {
        yield return new Position(X + 1, Y);

        if (X > 0)
        {
            yield return new Position(X - 1, Y);
        }

        yield return new Position(X, Y + 1);

        if (Y > 0)
        {
            yield return new Position(X, Y - 1);
        }
    }

    /// <summary>
    /// Возвращает все соседние клетки по ортогонали и диагонали.
    ///
    /// Полезно для более сложной логики, если в будущем
    /// потребуется анализировать ближайшее окружение шире, чем только
    /// клетки, достижимые обычным шагом.
    ///
    /// Метод не фильтрует клетки по границам арены.
    /// </summary>
    public IEnumerable<Position> GetNeighbors8()
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                {
                    continue;
                }

                int x = X + dx;
                int y = Y + dy;

                if (x < 0 || y < 0)
                {
                    continue;
                }

                yield return new Position(x, y);
            }
        }
    }
}
