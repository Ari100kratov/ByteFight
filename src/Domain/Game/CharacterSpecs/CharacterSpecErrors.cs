using SharedKernel;

namespace Domain.Game.CharacterSpecs;

public static class CharacterSpecErrors
{
    public static Error NotFound(Guid classId) => Error.NotFound(
        "CharacterSpecializations.NotFound",
        $"Специализация персонажа с Id = '{classId}' не найден");

    public static readonly Error NameNotUnique = Error.Conflict(
        "CharacterSpecializations.NameNotUnique",
        "Имя специализации персонажа уже занято");
}
