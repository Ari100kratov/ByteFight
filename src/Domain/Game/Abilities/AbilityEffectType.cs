using SharedKernel;

namespace Domain.Game.Abilities;

/// <summary>
/// Тип эффекта способности.
/// </summary>
[UserCodeApi]
public enum AbilityEffectType
{
    /// <summary>
    /// Способность наносит урон.
    /// </summary>
    Damage = 1,

    /// <summary>
    /// Способность восстанавливает здоровье.
    /// </summary>
    Healing = 2,

    /// <summary>
    /// Способность накладывает щит.
    /// </summary>
    Shield = 3,

    /// <summary>
    /// Способность накладывает положительный эффект.
    /// </summary>
    Buff = 4,

    /// <summary>
    /// Способность накладывает отрицательный эффект.
    /// </summary>
    Debuff = 5
}
