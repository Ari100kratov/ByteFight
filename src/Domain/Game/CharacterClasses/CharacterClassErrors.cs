using SharedKernel;

namespace Domain.Game.CharacterClasses;

public static class CharacterClassErrors
{
    public static Error NotFound(Guid classId) => Error.NotFound(
        "CharacterClasses.NotFound",
        $"Класс персонажа с Id = '{classId}' не найден");

    public static readonly Error NameNotUnique = Error.Conflict(
        "CharacterClasses.NameNotUnique",
        "Имя класс персонажа уже занято");
}
