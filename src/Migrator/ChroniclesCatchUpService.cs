using Chronicles.Application.Chronicles.ImportInbox;
using Chronicles.Application.Chronicles.ProcessCompletedGameSessions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Migrator;

internal sealed class ChroniclesCatchUpService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<ChroniclesCatchUpService> logger)
{
    public async Task CatchUpAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        int normalizedBatchSize = Math.Max(1, batchSize);
        int totalImported = 0;
        int totalProcessed = 0;

        while (true)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();

            IChroniclesInboxImportService inboxImportService = scope.ServiceProvider
                .GetRequiredService<IChroniclesInboxImportService>();

            IChroniclesProjectionService projectionService = scope.ServiceProvider
                .GetRequiredService<IChroniclesProjectionService>();

            int importedMessages = await inboxImportService.ImportNextBatchAsync(
                normalizedBatchSize,
                cancellationToken);

            ProcessCompletedGameSessionsResult result = await projectionService.ProcessNextBatchAsync(
                normalizedBatchSize,
                cancellationToken);

            if (importedMessages == 0 && result.ProcessedSessions == 0)
            {
                break;
            }

            totalImported += importedMessages;
            totalProcessed += result.ProcessedSessions;
        }

        MigratorLogMessages.ChroniclesCatchUpCompleted(
            logger,
            totalImported,
            totalProcessed);
    }
}
