namespace Domain.Game.Arenas.ArenaEnemies;

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
}

