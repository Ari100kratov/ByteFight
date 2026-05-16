namespace Chronicles.Domain;

/// <summary>
/// Справочник доступных номинаций хроник.
/// </summary>
public sealed class ChronicleNomination
{
    /// <summary>
    /// Идентификатор номинации.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Тип номинации.
    /// </summary>
    public ChronicleNominationType Type { get; set; }

    /// <summary>
    /// Машинное имя номинации.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Отображаемое имя номинации.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание номинации.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Подпись метрики в интерфейсе.
    /// </summary>
    public string MetricLabel { get; set; } = string.Empty;

    /// <summary>
    /// Единица измерения значения.
    /// </summary>
    public string MetricUnit { get; set; } = string.Empty;

    /// <summary>
    /// Тип значения для форматирования.
    /// </summary>
    public ChronicleNominationValueKind ValueKind { get; set; }

    /// <summary>
    /// Направление сортировки лидерборда.
    /// </summary>
    public ChronicleNominationSortDirection SortDirection { get; set; }
}
