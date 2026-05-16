using Chronicles.Application.Abstractions.Data;
using Chronicles.Domain;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Chronicles.Application.Chronicles.ProcessCompletedGameSessions;

internal sealed class ChroniclesProjectionService(
    IChroniclesDbContext chroniclesDbContext,
    ChroniclesProjectionSessionProcessor sessionProcessor,
    ICompletedGameSessionPayloadParser payloadParser,
    IDateTimeProvider dateTimeProvider,
    Microsoft.Extensions.Logging.ILogger<ChroniclesProjectionService> logger)
    : IChroniclesProjectionService
{
    private const int MaxProcessingAttempts = 5;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromMinutes(1);

    public async Task<ProcessCompletedGameSessionsResult> ProcessNextBatchAsync(int batchSize, CancellationToken cancellationToken)
    {
        DateTime now = dateTimeProvider.UtcNow;

        List<InboxMessage> inboxMessages = await chroniclesDbContext.InboxMessages
            .Where(x =>
                x.ProcessedAtUtc == null &&
                x.DeadLetteredAtUtc == null &&
                (x.NextAttemptAtUtc == null || x.NextAttemptAtUtc <= now))
            .OrderBy(x => x.ReceivedAtUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        int processedSessions = 0;
        int updatedCharacters = 0;
        int createdRecords = 0;

        foreach (InboxMessage inboxMessage in inboxMessages)
        {
            try
            {
                CompletedGameSessionData session = payloadParser.Parse(inboxMessage.Payload);
                ChroniclesProjectionSessionResult sessionResult = await sessionProcessor.ApplyAsync(session, cancellationToken);

                updatedCharacters += sessionResult.UpdatedCharacters;
                createdRecords += sessionResult.CreatedRecords;

                inboxMessage.ProcessedAtUtc = dateTimeProvider.UtcNow;
                inboxMessage.NextAttemptAtUtc = null;
                inboxMessage.Error = null;

                await chroniclesDbContext.SaveChangesAsync(cancellationToken);
                processedSessions++;
            }
            catch (Exception exception)
            {
                inboxMessage.AttemptCount++;
                inboxMessage.NextAttemptAtUtc = dateTimeProvider.UtcNow.Add(RetryDelay);
                inboxMessage.Error = exception.Message.Length > 2048 ? exception.Message[..2048] : exception.Message;

                if (inboxMessage.AttemptCount >= MaxProcessingAttempts)
                {
                    inboxMessage.DeadLetteredAtUtc = dateTimeProvider.UtcNow;
                    inboxMessage.NextAttemptAtUtc = null;
                }

                await chroniclesDbContext.SaveChangesAsync(cancellationToken);
            }
        }

        ChroniclesProjectionLogMessages.BatchProcessed(
            logger,
            processedSessions,
            updatedCharacters,
            createdRecords);

        return new ProcessCompletedGameSessionsResult(
            processedSessions,
            updatedCharacters,
            createdRecords);
    }
}
