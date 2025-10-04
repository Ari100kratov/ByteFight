using SharedKernel;

namespace Domain.Game.Characters;

public static class CharacterErrors
{
    public static Error NotFound(Guid characterId) => Error.NotFound(
        "Characters.NotFound",
        $"Персонаж с Id = '{characterId}' не найден");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Characters.NameNotUnique",
        "Имя персонажа уже занято");

    public static Error Unauthorized() => Error.Failure(
        "Characters.Unauthorized",
        "Вы не авторизованы для выполнения данного действия");
}
