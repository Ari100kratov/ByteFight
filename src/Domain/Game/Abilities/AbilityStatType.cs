namespace Domain.Game.Abilities;

/// <summary>
/// Тип характеристики способности.
/// </summary>
public enum AbilityStatType
{
    /// <summary>
    /// Дальность применения способности.
    /// </summary>
    Range = 1,

    /// <summary>
    /// Количество наносимого урона.
    /// </summary>
    Damage = 2,

    /// <summary>
    /// Количество восстанавливаемого здоровья.
    /// </summary>
    Healing = 3,

    /// <summary>
    /// Стоимость способности в мане.
    /// </summary>
    ManaCost = 4,

    /// <summary>
    /// Радиус области действия.
    /// </summary>
    AreaRadius = 5
}
