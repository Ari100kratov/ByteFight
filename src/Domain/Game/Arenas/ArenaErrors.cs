using SharedKernel;

namespace Domain.Game.Arenas;

public static class ArenaErrors
{
    public static Error NotFound(Guid arenaId) => Error.NotFound(
        "Arenas.NotFound",
        $"Арена с Id = '{arenaId}' не найдена");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Arenas.NameNotUnique",
        "Название арены уже занято");
}
