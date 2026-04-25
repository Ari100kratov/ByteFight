using Domain.Game.CharacterClasses;

namespace Application.Game.CharacterClasses.GetAll;

public sealed record ClassResponse(
    Guid Id,
    string Name,
    CharacterClassType Type,
    string? Description
);
