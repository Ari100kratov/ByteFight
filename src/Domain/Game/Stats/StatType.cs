using SharedKernel;

namespace Domain.Game.Stats;

/// <summary>
/// Типы характеристик.
/// </summary>
[UserCodeApi]
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
    /// Дальность перемещения.
    /// </summary>
    MoveRange = 3,

    /// <summary>
    /// Инициатива: определяет порядок ходов в раунде.
    /// </summary>
    Initiative = 4,

    /// <summary>
    /// Броня: плоское снижение получаемого урона.
    /// </summary>
    Armor = 5,

    /// <summary>
    /// Мана: ресурс применения способностей (восстанавливается каждый раунд).
    /// </summary>
    ManaRegen = 6,

    /// <summary>
    /// Мощь: процентная прибавка к исходящему урону и лечению.
    /// </summary>
    Power = 7,
}
