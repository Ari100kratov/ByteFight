using Domain.GameRuntime.RuntimeLogEntries;

namespace Domain.ValueObjects;

public sealed record Position
{
    public int X { get; init; }
    public int Y { get; init; }

    private Position() { } // Необходимо для EF

    public Position(int x, int y)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(x);
        ArgumentOutOfRangeException.ThrowIfNegative(y);

        X = x;
        Y = y;
    }

    public bool IsWithinGrid(int gridWidth, int gridHeight)
        => X >= 0 && X < gridWidth && Y >= 0 && Y < gridHeight;

    public int ManhattanDistance(Position position) => Math.Abs(X - position.X) + Math.Abs(Y - position.Y);

    public double EuclideanDistance(Position position) =>
        Math.Sqrt((X - position.X) * (X - position.X) + (Y - position.Y) * (Y - position.Y));

    public FacingDirection CalculateFacing(Position targetPosition) =>
        targetPosition.X < X ? FacingDirection.Left : FacingDirection.Right;

}

