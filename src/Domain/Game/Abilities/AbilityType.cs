using SharedKernel;

namespace Domain.Game.Abilities;

/// <summary>
/// Тип игровой способности юнита.
/// </summary>
[UserCodeApi]
public enum AbilityType
{
    /// <summary>
    /// Базовая ближняя атака.
    /// </summary>
    BasicMeleeAttack = 1,

    /// <summary>
    /// Базовая дальняя атака.
    /// </summary>
    BasicRangedAttack = 2,

    /// <summary>
    /// Способность лечения.
    /// </summary>
    Healing = 3
}
