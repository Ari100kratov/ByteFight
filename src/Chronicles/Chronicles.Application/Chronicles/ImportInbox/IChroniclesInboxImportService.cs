namespace Chronicles.Application.Chronicles.ImportInbox;

/// <summary>
/// Импортирует новые интеграционные сообщения из внешнего outbox в локальный inbox.
/// </summary>
public interface IChroniclesInboxImportService
{
    /// <summary>
    /// Импортирует следующую порцию сообщений.
    /// </summary>
    Task<int> ImportNextBatchAsync(int batchSize, CancellationToken cancellationToken);
}
