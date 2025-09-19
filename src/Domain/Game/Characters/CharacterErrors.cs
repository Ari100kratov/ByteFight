using SharedKernel;

namespace Domain.Game.Characters;

public static class CharacterErrors
{
    public static Error NotFound(Guid characterId) => Error.NotFound(
        "Characters.NotFound",
        $"The character with the Id = '{characterId}' was not found");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Characters.NameNotUnique",
        "The provided character name is not unique");

    public static Error Unauthorized() => Error.Failure(
        "Characters.Unauthorized",
        "You are not authorized to perform this action.");
}
