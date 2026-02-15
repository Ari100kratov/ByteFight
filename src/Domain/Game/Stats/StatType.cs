namespace Domain.Game.Stats;

/// <summary>
/// Типы характеристик.
/// </summary>
public enum StatType
{
    /// <summary>
    /// Текущее здоровье.
    /// </summary>
    Health = 1,

    /// <summary>
    /// Текущая мана.
    /// </summary>
    Mana = 2,

    /// <summary>
    /// Урон атаки.
    /// </summary>
    Attack = 3,

    /// <summary>
    /// Дальность атаки.
    /// </summary>
    AttackRange = 4,

    /// <summary>
    /// Дальность перемещения.
    /// </summary>
    MoveRange = 5,
}
