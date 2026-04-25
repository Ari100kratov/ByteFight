using Application.Contracts;
using Domain.Game.CharacterSpecs;

namespace Application.Game.CharacterSpecs.GetSpecsByClassId;

public sealed record SpecResponse(
    Guid Id,
    string Name,
    CharacterSpecType Type,
    string? Description,
    IReadOnlyList<StatDto> Stats,
    IReadOnlyList<ActionAssetDto> ActionAssets
);
