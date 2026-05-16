namespace Chronicles.Domain;

/// <summary>
/// Период агрегации хроник.
/// </summary>
public sealed class ChroniclePeriod
{
    /// <summary>
    /// Идентификатор периода.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Тип периода.
    /// </summary>
    public ChroniclePeriodType Type { get; set; }

    /// <summary>
    /// Момент начала периода.
    /// </summary>
    public DateTime StartsAtUtc { get; set; }

    /// <summary>
    /// Момент окончания периода.
    /// </summary>
    public DateTime? EndsAtUtc { get; set; }

    /// <summary>
    /// Признак того, что период закрыт и больше не пересчитывается.
    /// </summary>
    public bool IsClosed { get; set; }
}
