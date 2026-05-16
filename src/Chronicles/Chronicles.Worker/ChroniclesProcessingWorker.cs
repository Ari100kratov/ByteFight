using Chronicles.Application.Chronicles.ImportInbox;
using Chronicles.Application.Chronicles.ProcessCompletedGameSessions;
using Microsoft.Extensions.Options;

namespace Chronicles.Worker;

/// <summary>
/// Фоновый процессор, который последовательно импортирует интеграционные события и обновляет витрину хроник.
/// </summary>
public sealed class ChroniclesProcessingWorker(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<ChroniclesWorkerOptions> options,
    ILogger<ChroniclesProcessingWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ChroniclesWorkerOptions workerOptions = options.Value;
        var pollInterval = TimeSpan.FromSeconds(Math.Max(1, workerOptions.PollIntervalSeconds));

        using var timer = new PeriodicTimer(pollInterval);

        do
        {
            await ProcessBatchAsync(workerOptions.BatchSize, stoppingToken);
        }
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task ProcessBatchAsync(int batchSize, CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();

        IChroniclesInboxImportService inboxImportService = scope.ServiceProvider
            .GetRequiredService<IChroniclesInboxImportService>();

        IChroniclesProjectionService projectionService = scope.ServiceProvider
            .GetRequiredService<IChroniclesProjectionService>();

        try
        {
            int importedMessages = await inboxImportService.ImportNextBatchAsync(batchSize, cancellationToken);
            ProcessCompletedGameSessionsResult result = await projectionService.ProcessNextBatchAsync(batchSize, cancellationToken);

            if (importedMessages > 0 || result.ProcessedSessions > 0)
            {
                ChroniclesWorkerLogMessages.BatchCompleted(logger, importedMessages, result.ProcessedSessions);
            }
        }
        catch (Exception exception)
        {
            ChroniclesWorkerLogMessages.BatchFailed(logger, exception);
        }
    }
}
