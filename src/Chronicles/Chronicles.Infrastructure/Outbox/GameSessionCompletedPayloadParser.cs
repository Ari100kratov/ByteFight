using System.Text.Json;
using Chronicles.Application.Abstractions.Data;
using Chronicles.Application.Chronicles.ProcessCompletedGameSessions;
using IntegrationContracts;
using IntegrationContracts.GameSessions;

namespace Chronicles.Infrastructure.Outbox;

internal sealed class GameSessionCompletedPayloadParser : ICompletedGameSessionPayloadParser
{
    public CompletedGameSessionData Parse(string payload)
    {
        GameSessionCompletedIntegrationEvent integrationEvent =
            JsonSerializer.Deserialize<GameSessionCompletedIntegrationEvent>(
                payload,
                IntegrationEventJson.SerializerOptions)
            ?? throw new InvalidOperationException("Inbox payload cannot be deserialized.");

        if (integrationEvent.SchemaVersion != 1)
        {
            throw new InvalidOperationException($"Unsupported integration event schema version '{integrationEvent.SchemaVersion}'.");
        }

        return new CompletedGameSessionData(
            integrationEvent.SessionId,
            integrationEvent.TotalTurns,
            integrationEvent.StartedAtUtc,
            integrationEvent.EndedAtUtc,
            integrationEvent.Outcome switch
            {
                IntegrationGameOutcome.Victory => CompletedGameSessionOutcome.Victory,
                IntegrationGameOutcome.Defeat => CompletedGameSessionOutcome.Defeat,
                IntegrationGameOutcome.Draw => CompletedGameSessionOutcome.Draw,
                IntegrationGameOutcome.TimeoutLoss => CompletedGameSessionOutcome.TimeoutLoss,
                IntegrationGameOutcome.TurnLimitLoss => CompletedGameSessionOutcome.TurnLimitLoss,
                _ => throw new InvalidOperationException($"Unsupported integration outcome '{integrationEvent.Outcome}'.")
            },
            integrationEvent.WinnerUnitId,
            integrationEvent.Participants.Select(participant => new CompletedGameSessionParticipantData(
                participant.UnitId,
                participant.UserId,
                participant.UnitType switch
                {
                    IntegrationParticipantUnitType.Player => CompletedParticipantUnitType.Player,
                    IntegrationParticipantUnitType.Npc => CompletedParticipantUnitType.Npc,
                    _ => throw new InvalidOperationException($"Unsupported participant unit type '{participant.UnitType}'.")
                },
                participant.DisplayName,
                participant.UserFirstName,
                participant.UserLastName,
                participant.CharacterClassName,
                participant.CharacterSpecName)).ToList(),
            integrationEvent.Logs.Select(MapLogEntry).ToList());
    }

    private static CompletedGameSessionLogEntryData MapLogEntry(GameActionLogEntrySnapshot logEntry) =>
        logEntry switch
        {
            IdleLogEntrySnapshot idle => new CompletedGameSessionLogEntryData(
                idle.ActorId,
                CompletedGameSessionLogEntryKind.Idle,
                idle.EntryType,
                idle.TurnIndex,
                null,
                0,
                null,
                null),
            WalkLogEntrySnapshot walk => new CompletedGameSessionLogEntryData(
                walk.ActorId,
                CompletedGameSessionLogEntryKind.Walk,
                walk.EntryType,
                walk.TurnIndex,
                null,
                0,
                null,
                null),
            DeathLogEntrySnapshot death => new CompletedGameSessionLogEntryData(
                death.ActorId,
                CompletedGameSessionLogEntryKind.Death,
                death.EntryType,
                death.TurnIndex,
                null,
                0,
                null,
                null),
            AbilityUsedLogEntrySnapshot ability => new CompletedGameSessionLogEntryData(
                ability.ActorId,
                CompletedGameSessionLogEntryKind.AbilityUsed,
                ability.EntryType,
                ability.TurnIndex,
                ability.TargetId,
                ability.Value,
                ability.EffectType,
                null),
            ItemPickedUpLogEntrySnapshot item => new CompletedGameSessionLogEntryData(
                item.ActorId,
                CompletedGameSessionLogEntryKind.ItemPickedUp,
                item.EntryType,
                item.TurnIndex,
                null,
                item.Value,
                null,
                item.ItemType),
            StatusAppliedLogEntrySnapshot status => new CompletedGameSessionLogEntryData(
                status.ActorId,
                CompletedGameSessionLogEntryKind.StatusApplied,
                status.EntryType,
                status.TurnIndex,
                status.TargetId,
                status.Magnitude,
                null,
                null),
            RoundStartedLogEntrySnapshot round => new CompletedGameSessionLogEntryData(
                round.ActorId,
                CompletedGameSessionLogEntryKind.RoundStarted,
                round.EntryType,
                round.TurnIndex,
                null,
                0,
                null,
                null),
            _ => throw new InvalidOperationException($"Unsupported integration log entry '{logEntry.GetType().Name}'.")
        };
}
