using Application.Contracts;
using Domain.Game.CharacterSpecs;

namespace Application.Game.Characters.GetDetails;

public sealed record CharacterResponse(Guid Id, string Name, SpecResponse Spec);

public sealed record SpecResponse(
    Guid Id,
    string Name,
    string ClassName,
    CharacterSpecType Type,
    string? Description,
    IReadOnlyList<StatDto> Stats,
    IReadOnlyList<ActionAssetDto> ActionAssets
);

