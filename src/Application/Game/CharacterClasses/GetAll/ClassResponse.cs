using Application.Game.Common.Dtos;

namespace Application.Game.CharacterClasses.GetAll;

public sealed record ClassResponse(
    Guid Id,
    string Name,
    string? Description,
    IReadOnlyList<StatDto> Stats,
    IReadOnlyList<ActionAssetDto> ActionAssets
);
