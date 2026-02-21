using Domain.ValueObjects;

namespace GameRuntime.Logic.User.Api;

public sealed record UserArenaDefinition
{
    public required int GridWidth { get; init; }
    public required int GridHeight { get; init; }
    public required Position StartPosition { get; init; }
    public required IReadOnlyList<Position> BlockedPositions { get; init; }
}
