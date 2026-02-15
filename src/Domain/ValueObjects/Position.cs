using Domain.GameRuntime.RuntimeLogEntries;

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
}
