namespace GameRuntime.Core;

public sealed record ArenaDefinition
{
    public required Guid ArenaId { get; init; }

    public required int GridWidth { get; init; }

    public required int GridHeight { get; init; }

    public int MaxTurnsCount { get; } = 100;
}
