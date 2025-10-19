using SharedKernel;

namespace Domain.Game.Arenas;

public static class ArenaErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Arena.NotFound", $"Арена с Id = {id} не найдена.");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Arenas.NameNotUnique",
        "Название арены уже занято");
}
