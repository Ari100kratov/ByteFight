namespace Chronicles.Worker;

/// <summary>
/// Настройки фонового процессора хроник.
/// </summary>
public sealed class ChroniclesWorkerOptions
{
    /// <summary>
    /// Имя конфигурационной секции.
    /// </summary>
    public const string SectionName = "ChroniclesWorker";

    /// <summary>
    /// Размер одной порции обрабатываемых игровых сессий.
    /// </summary>
    public int BatchSize { get; init; } = 100;

    /// <summary>
    /// Интервал опроса источника завершённых игровых сессий.
    /// </summary>
    public int PollIntervalSeconds { get; init; } = 300;
}
