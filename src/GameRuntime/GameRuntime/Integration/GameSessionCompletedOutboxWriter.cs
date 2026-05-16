using System.Text.Json;
using Application.Abstractions.Data;
using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.GameRuntime.GameSessions;
using Domain.Integration;
using IntegrationContracts;
using IntegrationContracts.GameSessions;
using Microsoft.EntityFrameworkCore;

namespace GameRuntime.Integration;

internal sealed class GameSessionCompletedOutboxWriter(
    IGameRuntimeDbContext dbContext,
    IGameSessionCompletedIntegrationEventFactory integrationEventFactory)
    : IGameSessionCompletedOutboxWriter
{
    public async Task EnqueueAsync(GameSession session, CancellationToken cancellationToken)
    {
        List<GameActionLogEntry> logs = await dbContext.GameActionLogEntries
            .AsNoTracking()
            .Where(x => x.SessionId == session.Id)
            .OrderBy(x => x.TurnIndex)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        GameSessionCompletedIntegrationEvent integrationEvent = integrationEventFactory.Create(session, logs);

        dbContext.OutboxMessages.Add(new OutboxMessage
        {
            Id = integrationEvent.EventId,
            AggregateId = session.Id,
            Type = nameof(GameSessionCompletedIntegrationEvent),
            Payload = JsonSerializer.Serialize(integrationEvent, IntegrationEventJson.SerializerOptions),
            OccurredAtUtc = integrationEvent.EndedAtUtc,
            CreatedAtUtc = integrationEvent.CreatedAtUtc
        });
    }
}
