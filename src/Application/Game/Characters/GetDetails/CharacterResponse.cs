using Application.Contracts;
using Domain.Game.CharacterClasses;

namespace Application.Game.Characters.GetDetails;

public sealed record CharacterResponse(Guid Id, string Name, ClassResponse Class);

public sealed record ClassResponse(
    Guid Id,
    string Name,
    CharacterClassType Type,
    string? Description,
    IReadOnlyList<StatDto> Stats,
    IReadOnlyList<ActionAssetDto> ActionAssets
);

