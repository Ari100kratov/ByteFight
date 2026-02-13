namespace Domain.Game.Stats;

/// <summary>
/// Типы характеристик, доступных в боевом движке и пользовательских скриптах.
/// </summary>
public enum StatType
{
    /// <summary>
    /// Текущее здоровье юнита.
    /// </summary>
    Health = 1,

    /// <summary>
    /// Текущая мана юнита.
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
