namespace Chronicles.Application.Chronicles.ProcessCompletedGameSessions;

/// <summary>
/// Выполняет последовательную обработку завершённых игровых сессий и обновляет хроники.
/// </summary>
public interface IChroniclesProjectionService
{
    /// <summary>
    /// Обрабатывает следующую порцию завершённых игровых сессий.
    /// </summary>
    Task<ProcessCompletedGameSessionsResult> ProcessNextBatchAsync(int batchSize, CancellationToken cancellationToken);
}
