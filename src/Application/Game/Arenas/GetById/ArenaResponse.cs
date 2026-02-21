using Application.Contracts;

namespace Application.Game.Arenas.GetById;

public sealed record ArenaResponse(
    Guid Id,
    string Name,
    int GridWidth,
    int GridHeight,
    string? BackgroundAsset,
    string? Description,
    PositionDto StartPosition,
    PositionDto[] BlockedPositions
);
