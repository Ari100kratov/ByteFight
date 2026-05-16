namespace Chronicles.Domain;

/// <summary>
/// Сообщение inbox для обработки интеграционных событий.
/// </summary>
public sealed class InboxMessage
{
    /// <summary>
    /// Идентификатор события.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Тип сообщения.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Полезная нагрузка в JSON.
    /// </summary>
    public string Payload { get; set; } = string.Empty;

    /// <summary>
    /// Время получения сообщения.
    /// </summary>
    public DateTime ReceivedAtUtc { get; set; }

    /// <summary>
    /// Количество неуспешных попыток локальной обработки.
    /// </summary>
    public int AttemptCount { get; set; }

    /// <summary>
    /// Время следующей разрешённой попытки обработки.
    /// </summary>
    public DateTime? NextAttemptAtUtc { get; set; }

    /// <summary>
    /// Время успешной обработки сообщения.
    /// </summary>
    public DateTime? ProcessedAtUtc { get; set; }

    /// <summary>
    /// Время перевода сообщения в dead-letter.
    /// </summary>
    public DateTime? DeadLetteredAtUtc { get; set; }

    /// <summary>
    /// Последняя ошибка обработки.
    /// </summary>
    public string? Error { get; set; }
}
