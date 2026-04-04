using Domain.ValueObjects;

namespace GameRuntime.Common.World;

public sealed record ArenaDefinition
{
    public required Guid ArenaId { get; init; }
    public required int GridWidth { get; init; }
    public required int GridHeight { get; init; }
    public required Position StartPosition { get; init; }
    public required Position[] BlockedPositions { get; init; }

    public int MaxTurnsCount { get; } = 100;
}
