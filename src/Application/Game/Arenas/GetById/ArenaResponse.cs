using Application.Contracts;
using Domain.Game.ArenaItems;

namespace Application.Game.Arenas.GetById;

public sealed record ArenaResponse(
    Guid Id,
    string Name,
    int GridWidth,
    int GridHeight,
    string BackgroundAsset,
    string? Description,
    PositionDto StartPosition,
    PositionDto[] BlockedPositions,
    ArenaItemResponse[] Items);

public sealed record ArenaItemResponse(
    Guid PlacedItemId,
    Guid ItemId,
    string Name,
    string? Description,
    ArenaItemType Type,
    int Value,
    PositionDto Position,
    SpriteAnimationDto Sprite);
