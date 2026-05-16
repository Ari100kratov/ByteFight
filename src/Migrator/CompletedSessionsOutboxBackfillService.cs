using System.Text.Json;
using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.GameRuntime.GameSessionParticipants;
using Domain.GameRuntime.GameSessions;
using Domain.Integration;
using GameRuntime.Integration;
using Infrastructure.Database.Auth;
using Infrastructure.Database.Game;
using Infrastructure.Database.GameRuntime;
using IntegrationContracts;
using IntegrationContracts.GameSessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Migrator;

internal sealed class CompletedSessionsOutboxBackfillService(
    GameRuntimeDbContext gameRuntimeDbContext,
    GameDbContext gameDbContext,
    AuthDbContext authDbContext,
    IGameSessionCompletedIntegrationEventFactory eventFactory,
    ILogger<CompletedSessionsOutboxBackfillService> logger)
{
    private const int BatchSize = 100;
    private const string MessageType = nameof(GameSessionCompletedIntegrationEvent);

    public async Task BackfillAsync(CancellationToken cancellationToken = default)
    {
        int totalBackfilled = 0;

        while (true)
        {
            List<GameSession> sessions = await LoadNextBatchAsync(cancellationToken);

            if (sessions.Count == 0)
            {
                break;
            }

            await FillMissingParticipantSnapshotsAsync(sessions, cancellationToken);

            Guid[] sessionIds = [.. sessions.Select(x => x.Id)];
            List<GameActionLogEntry> logs = await gameRuntimeDbContext.GameActionLogEntries
                .AsNoTracking()
                .Where(x => sessionIds.Contains(x.SessionId))
                .OrderBy(x => x.TurnIndex)
                .ThenBy(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            ILookup<Guid, GameActionLogEntry> logsBySessionId = logs.ToLookup(x => x.SessionId);

            foreach (GameSession session in sessions)
            {
                GameSessionCompletedIntegrationEvent integrationEvent = eventFactory.Create(
                    session,
                    [.. logsBySessionId[session.Id]]);

                gameRuntimeDbContext.OutboxMessages.Add(new OutboxMessage
                {
                    Id = integrationEvent.EventId,
                    AggregateId = session.Id,
                    Type = MessageType,
                    Payload = JsonSerializer.Serialize(integrationEvent, IntegrationEventJson.SerializerOptions),
                    OccurredAtUtc = integrationEvent.EndedAtUtc,
                    CreatedAtUtc = integrationEvent.CreatedAtUtc
                });
            }

            await gameRuntimeDbContext.SaveChangesAsync(cancellationToken);
            totalBackfilled += sessions.Count;
        }

        MigratorLogMessages.CompletedSessionsBackfilled(logger, totalBackfilled);
    }

    private async Task<List<GameSession>> LoadNextBatchAsync(CancellationToken cancellationToken) =>
        await gameRuntimeDbContext.GameSessions
            .Where(x => x.Status == GameStatus.Completed && x.EndedAt != null)
            .Where(x => !gameRuntimeDbContext.OutboxMessages.Any(outbox =>
                outbox.AggregateId == x.Id &&
                outbox.Type == MessageType))
            .OrderBy(x => x.EndedAt)
            .ThenBy(x => x.Id)
            .Include(x => x.Participants)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

    private async Task FillMissingParticipantSnapshotsAsync(
        IReadOnlyCollection<GameSession> sessions,
        CancellationToken cancellationToken)
    {
        GameSessionParticipant[] participantsForSnapshot =
        [
            .. sessions
                .SelectMany(x => x.Participants)
                .Where(x =>
                    string.IsNullOrWhiteSpace(x.UnitName) ||
                    x.UnitType == ParticipantUnitType.Player &&
                    (string.IsNullOrWhiteSpace(x.UserFirstName) ||
                     string.IsNullOrWhiteSpace(x.UserLastName) ||
                     string.IsNullOrWhiteSpace(x.CharacterClassName) ||
                     string.IsNullOrWhiteSpace(x.CharacterSpecName)))
        ];

        if (participantsForSnapshot.Length == 0)
        {
            return;
        }

        Guid[] characterIds =
        [
            .. participantsForSnapshot
                .Where(x => x.UnitType == ParticipantUnitType.Player)
                .Select(x => x.UnitId.Value)
                .Distinct()
        ];

        Guid[] arenaEnemyIds =
        [
            .. participantsForSnapshot
                .Where(x => x.UnitType == ParticipantUnitType.Npc)
                .Select(x => x.UnitId.Value)
                .Distinct()
        ];

        Guid[] userIds =
        [
            .. participantsForSnapshot
                .Where(x => x.UserId is not null)
                .Select(x => x.UserId!.Value.Value)
                .Distinct()
        ];

        Dictionary<Guid, CharacterSnapshot> characters = await gameDbContext.Characters
            .AsNoTracking()
            .Where(x => characterIds.Contains(x.Id))
            .Select(x => new
            {
                x.Id,
                x.Name,
                ClassName = x.Spec.Class.Name,
                SpecName = x.Spec.Name
            })
            .ToDictionaryAsync(
                x => x.Id,
                x => new CharacterSnapshot(x.Name, x.ClassName, x.SpecName),
                cancellationToken);

        Dictionary<Guid, UserSnapshot> users = await authDbContext.Users
            .AsNoTracking()
            .Where(x => userIds.Contains(x.Id))
            .Select(x => new
            {
                x.Id,
                x.FirstName,
                x.LastName
            })
            .ToDictionaryAsync(
                x => x.Id,
                x => new UserSnapshot(x.FirstName, x.LastName),
                cancellationToken);

        Dictionary<Guid, string> arenaEnemyNames = await gameDbContext.ArenaEnemies
            .AsNoTracking()
            .Where(x => arenaEnemyIds.Contains(x.Id))
            .Include(x => x.Enemy)
            .ToDictionaryAsync(x => x.Id, x => x.Enemy.Name, cancellationToken);

        foreach (GameSessionParticipant participant in participantsForSnapshot)
        {
            if (participant.UnitType == ParticipantUnitType.Player)
            {
                characters.TryGetValue(participant.UnitId.Value, out CharacterSnapshot? character);

                if (string.IsNullOrWhiteSpace(participant.UnitName))
                {
                    participant.UnitName = character?.Name ?? "Unknown character";
                }

                participant.CharacterClassName = character?.ClassName;
                participant.CharacterSpecName = character?.SpecName;

                if (participant.UserId is not null &&
                    users.TryGetValue(participant.UserId.Value.Value, out UserSnapshot? user))
                {
                    participant.UserFirstName = user.FirstName;
                    participant.UserLastName = user.LastName;
                }

                continue;
            }

            if (string.IsNullOrWhiteSpace(participant.UnitName))
            {
                participant.UnitName = participant.UnitType switch
                {
                    ParticipantUnitType.Npc => arenaEnemyNames.GetValueOrDefault(participant.UnitId.Value, "Unknown enemy"),
                    _ => participant.UnitId.Value.ToString()
                };
            }
        }
    }

    private sealed record CharacterSnapshot(string Name, string? ClassName, string? SpecName);

    private sealed record UserSnapshot(string FirstName, string LastName);
}
