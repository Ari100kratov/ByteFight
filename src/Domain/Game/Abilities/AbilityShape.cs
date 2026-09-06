using SharedKernel;

namespace Domain.Game.Abilities;

/// <summary>
/// Форма области действия способности на гексагональном поле.
/// </summary>
[UserCodeApi]
public enum AbilityShape
{
    /// <summary>
    /// Одиночная цель: конкретный юнит или гекс.
    /// </summary>
    SingleTarget = 1,

    /// <summary>
    /// Радиус: все гексы в радиусе от точки применения.
    /// </summary>
    Radius = 2,

    /// <summary>
    /// Прямая линия от применяющего через точку применения.
    /// </summary>
    Line = 3,

    /// <summary>
    /// Клин (сектор из трёх смежных направлений) от применяющего.
    /// </summary>
    Cone = 4
}
