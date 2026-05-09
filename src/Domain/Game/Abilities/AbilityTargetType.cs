using SharedKernel;

namespace Domain.Game.Abilities;

/// <summary>
/// Тип цели способности.
/// </summary>
[UserCodeApi]
public enum AbilityTargetType
{
    /// <summary>
    /// Целью является противник.
    /// </summary>
    Enemy = 1,

    /// <summary>
    /// Целью является сам применяющий юнит.
    /// </summary>
    Self = 2,

    /// <summary>
    /// Целью является союзник.
    /// </summary>
    Ally = 3,

    /// <summary>
    /// Целью является область на поле.
    /// </summary>
    Area = 4
}
