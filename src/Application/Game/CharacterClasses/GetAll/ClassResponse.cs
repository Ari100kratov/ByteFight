using Application.Game.Common.Dtos;
using Domain.Game.CharacterClasses;

namespace Application.Game.CharacterClasses.GetAll;

public sealed record ClassResponse(
    Guid Id,
    string Name,
    CharacterClassType Type,
    string? Description,
    IReadOnlyList<StatDto> Stats,
    IReadOnlyList<ActionAssetDto> ActionAssets
);
