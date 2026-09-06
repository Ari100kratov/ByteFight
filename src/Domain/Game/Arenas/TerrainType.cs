namespace Domain.Game.Arenas;

/// <summary>
/// Тип рельефа гекса арены. Влияет на стоимость входа,
/// проходимость и защиту находящегося на нём юнита.
/// </summary>
public enum TerrainType
{
    /// <summary>Луговина: обычная клетка, стоимость входа 1.</summary>
    Meadow = 1,

    /// <summary>Чащоба: стоимость входа 2, +20% защиты от урона.</summary>
    Forest = 2,

    /// <summary>Топь: стоимость входа 2, входящий теряет защиту.</summary>
    Swamp = 3,

    /// <summary>Вода: непроходима для большинства юнитов.</summary>
    Water = 4,

    /// <summary>Камень: непроходима и блокирует линию видимости.</summary>
    Rock = 5
}

/// <summary>
/// Правила влияния рельефа на бой.
/// </summary>
public static class TerrainRules
{
    /// <summary>
    /// Стоимость входа в гекс в очках перемещения.
    /// Непроходимому рельефу соответствует бесконечная стоимость.
    /// </summary>
    public static int MovementCost(TerrainType terrain) => terrain switch
    {
        TerrainType.Meadow => 1,
        TerrainType.Forest => 2,
        TerrainType.Swamp => 2,
        _ => int.MaxValue
    };

    /// <summary>
    /// Проходим ли рельеф для пешего юнита.
    /// </summary>
    public static bool IsPassable(TerrainType terrain) =>
        MovementCost(terrain) != int.MaxValue;

    /// <summary>
    /// Блокирует ли рельеф линии видимости (для дальних способностей).
    /// </summary>
    public static bool BlocksLineOfSight(TerrainType terrain) =>
        terrain == TerrainType.Rock;

    /// <summary>
    /// Множитель входящего урона для юнита, стоящего на рельефе
    /// (1.0 — без изменений, меньше — защита).
    /// </summary>
    public static decimal IncomingDamageMultiplier(TerrainType terrain) => terrain switch
    {
        TerrainType.Forest => 0.8m,
        TerrainType.Swamp => 1.25m,
        _ => 1.0m
    };
}
