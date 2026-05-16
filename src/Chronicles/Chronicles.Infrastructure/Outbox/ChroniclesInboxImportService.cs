using Chronicles.Application.Chronicles.ImportInbox;
using Chronicles.Domain;
using Chronicles.Infrastructure.Database;
using Chronicles.Infrastructure.Source.GameRuntime;
using IntegrationContracts.GameSessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SharedKernel;

namespace Chronicles.Infrastructure.Outbox;

internal sealed class ChroniclesInboxImportService(
    ChroniclesDbContext chroniclesDbContext,
    SourceGameRuntimeReadDbContext sourceGameRuntimeReadDbContext,
    IDateTimeProvider dateTimeProvider)
    : IChroniclesInboxImportService
{
    private const string CursorName = "game_runtime_outbox";
    private const string MessageType = nameof(GameSessionCompletedIntegrationEvent);

    public async Task<int> ImportNextBatchAsync(int batchSize, CancellationToken cancellationToken)
    {
        OutboxImportCursor? cursor = await chroniclesDbContext.ImportCursors
            .SingleOrDefaultAsync(x => x.Name == CursorName, cancellationToken);

        List<OutboxMessageReadModel> messages = await LoadNextOutboxMessagesAsync(
            cursor,
            batchSize,
            cancellationToken);

        if (messages.Count == 0)
        {
            return 0;
        }

        Guid[] messageIds = [.. messages.Select(x => x.Id)];
        HashSet<Guid> existingInboxMessageIds = await chroniclesDbContext.InboxMessages
            .Where(x => messageIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToHashSetAsync(cancellationToken);

        int importedMessages = 0;

        await using IDbContextTransaction transaction =
            await chroniclesDbContext.Database.BeginTransactionAsync(cancellationToken);

        foreach (OutboxMessageReadModel message in messages)
        {
            if (!existingInboxMessageIds.Contains(message.Id))
            {
                chroniclesDbContext.InboxMessages.Add(new InboxMessage
                {
                    Id = message.Id,
                    Type = message.Type,
                    Payload = message.Payload,
                    ReceivedAtUtc = dateTimeProvider.UtcNow
                });
                importedMessages++;
            }

            cursor ??= new OutboxImportCursor
            {
                Name = CursorName
            };

            cursor.LastMessageCreatedAtUtc = message.CreatedAtUtc;
            cursor.LastMessageId = message.Id;
            cursor.UpdatedAtUtc = dateTimeProvider.UtcNow;
        }

        if (cursor is not null && chroniclesDbContext.Entry(cursor).State == EntityState.Detached)
        {
            chroniclesDbContext.ImportCursors.Add(cursor);
        }

        await chroniclesDbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return importedMessages;
    }

    private async Task<List<OutboxMessageReadModel>> LoadNextOutboxMessagesAsync(
        OutboxImportCursor? cursor,
        int batchSize,
        CancellationToken cancellationToken)
    {
        if (cursor is null)
        {
            return await sourceGameRuntimeReadDbContext.OutboxMessages
                .FromSqlInterpolated($"""
                    SELECT id, aggregate_id, type, payload, occurred_at_utc, created_at_utc, processed_at_utc, error
                    FROM integration.outbox_messages
                    WHERE type = {MessageType}
                    ORDER BY created_at_utc, id
                    LIMIT {batchSize}
                    """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        return await sourceGameRuntimeReadDbContext.OutboxMessages
            .FromSqlInterpolated($"""
                SELECT id, aggregate_id, type, payload, occurred_at_utc, created_at_utc, processed_at_utc, error
                FROM integration.outbox_messages
                WHERE type = {MessageType}
                  AND (created_at_utc, id) > ({cursor.LastMessageCreatedAtUtc}, {cursor.LastMessageId})
                ORDER BY created_at_utc, id
                LIMIT {batchSize}
                """)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
