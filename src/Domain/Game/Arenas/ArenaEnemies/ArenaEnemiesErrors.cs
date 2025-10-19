using SharedKernel;

namespace Domain.Game.Arenas.ArenaEnemies;

public static class ArenaEnemiesErrors
{
    public static Error InvalidCoordinates(int x, int y) =>
        Error.Problem("ArenaEnemies.InvalidCoordinates", $"Координаты ({x},{y}) вне границ арены.");

    public static Error CellOccupied(int x, int y) =>
        Error.Conflict("ArenaEnemies.CellOccupied", $"Клетка ({x},{y}) уже занята.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("ArenaEnemies.NotFound", $"Противник с идентификатором {id} не найден на арене.");
}
