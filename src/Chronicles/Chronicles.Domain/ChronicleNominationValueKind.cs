namespace Chronicles.Domain;

/// <summary>
/// Тип отображаемого значения номинации.
/// </summary>
public enum ChronicleNominationValueKind
{
    /// <summary>
    /// Значение отображается как количество ходов.
    /// </summary>
    Turns = 1,

    /// <summary>
    /// Значение отображается как счётчик.
    /// </summary>
    Count = 2,

    /// <summary>
    /// Значение отображается как количество урона.
    /// </summary>
    Damage = 3,

    /// <summary>
    /// Значение отображается как количество лечения.
    /// </summary>
    Healing = 4
}
