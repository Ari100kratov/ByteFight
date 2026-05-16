namespace Chronicles.Domain;

/// <summary>
/// Направление сортировки лидерборда номинации.
/// </summary>
public enum ChronicleNominationSortDirection
{
    /// <summary>
    /// Лучшим считается минимальное значение.
    /// </summary>
    Ascending = 1,

    /// <summary>
    /// Лучшим считается максимальное значение.
    /// </summary>
    Descending = 2
}
