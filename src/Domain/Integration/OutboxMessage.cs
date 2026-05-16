namespace Domain.Integration;

/// <summary>
/// Сообщение outbox для межконтекстной интеграции.
/// </summary>
public sealed class OutboxMessage
{
    /// <summary>
    /// Идентификатор сообщения.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор агрегата, породившего событие.
    /// </summary>
    public Guid AggregateId { get; set; }

    /// <summary>
    /// Тип сообщения.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Полезная нагрузка в JSON.
    /// </summary>
    public string Payload { get; set; } = string.Empty;

    /// <summary>
    /// Время возникновения сообщения.
    /// </summary>
    public DateTime OccurredAtUtc { get; set; }

    /// <summary>
    /// Время записи сообщения в outbox.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Время успешной публикации сообщения.
    /// </summary>
    public DateTime? ProcessedAtUtc { get; set; }

    /// <summary>
    /// Последняя ошибка публикации.
    /// </summary>
    public string? Error { get; set; }
}
