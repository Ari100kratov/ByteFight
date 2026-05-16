using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.GameRuntime.GameResults;
using Domain.GameRuntime.GameSessions;
using IntegrationContracts.GameSessions;
using SharedKernel;

namespace GameRuntime.Integration;

public sealed class GameSessionCompletedIntegrationEventFactory(IDateTimeProvider dateTimeProvider)
    : IGameSessionCompletedIntegrationEventFactory
{
    public GameSessionCompletedIntegrationEvent Create(GameSession session, IReadOnlyList<GameActionLogEntry> logs)
    {
        return new GameSessionCompletedIntegrationEvent
        {
            SchemaVersion = 1,
            EventId = Guid.CreateVersion7(),
            SessionId = session.Id,
            CreatedAtUtc = dateTimeProvider.UtcNow,
            ArenaId = session.ArenaId.Value,
            StartedAtUtc = session.StartedAt,
            EndedAtUtc = session.EndedAt ?? throw new InvalidOperationException("Completed session must have EndedAt."),
            TotalTurns = session.TotalTurns,
            Outcome = MapOutcome(session.Result?.Outcome ?? throw new InvalidOperationException("Completed session must have result.")),
            WinnerUnitId = session.Result?.WinnerUnitId?.Value,
            Participants = session.Participants
                .Select(x => new GameSessionParticipantSnapshot(
                    x.UnitId.Value,
                    x.UserId?.Value,
                    x.UnitType switch
                    {
                        Domain.GameRuntime.GameSessionParticipants.ParticipantUnitType.Player => IntegrationParticipantUnitType.Player,
                        Domain.GameRuntime.GameSessionParticipants.ParticipantUnitType.Npc => IntegrationParticipantUnitType.Npc,
                        _ => throw new InvalidOperationException($"Unsupported participant unit type '{x.UnitType}'.")
                    },
                    x.UnitName,
                    x.UserFirstName,
                    x.UserLastName,
                    x.CharacterClassName,
                    x.CharacterSpecName))
                .ToList(),
            Logs = logs.Select(MapLogEntry).ToList()
        };
    }

    private static IntegrationGameOutcome MapOutcome(GameOutcome outcome) =>
        outcome switch
        {
            GameOutcome.Victory => IntegrationGameOutcome.Victory,
            GameOutcome.Defeat => IntegrationGameOutcome.Defeat,
            GameOutcome.Draw => IntegrationGameOutcome.Draw,
            GameOutcome.TimeoutLoss => IntegrationGameOutcome.TimeoutLoss,
            GameOutcome.TurnLimitLoss => IntegrationGameOutcome.TurnLimitLoss,
            _ => throw new ArgumentOutOfRangeException(nameof(outcome), outcome, null)
        };

    private static GameActionLogEntrySnapshot MapLogEntry(GameActionLogEntry logEntry) =>
        logEntry switch
        {
            AbilityUsedLogEntry ability => new AbilityUsedLogEntrySnapshot(
                ability.Id,
                ability.SessionId,
                ability.ActorId.Value,
                ability.ActorName,
                ability.EntryType.ToString(),
                ability.Info,
                ability.TurnIndex,
                ability.CreatedAt,
                ability.AbilityType.ToString(),
                ability.EffectType.ToString(),
                ability.AbilityName,
                ability.TargetId.Value,
                ability.TargetName,
                ability.Value,
                ability.FacingDirection.ToString(),
                new StatSnapshotContract(ability.TargetHp.Current, ability.TargetHp.Max)),
            WalkLogEntry walk => new WalkLogEntrySnapshot(
                walk.Id,
                walk.SessionId,
                walk.ActorId.Value,
                walk.ActorName,
                walk.EntryType.ToString(),
                walk.Info,
                walk.TurnIndex,
                walk.CreatedAt,
                walk.FacingDirection.ToString(),
                new PositionSnapshot(walk.To.X, walk.To.Y)),
            DeathLogEntry death => new DeathLogEntrySnapshot(
                death.Id,
                death.SessionId,
                death.ActorId.Value,
                death.ActorName,
                death.EntryType.ToString(),
                death.Info,
                death.TurnIndex,
                death.CreatedAt),
            IdleLogEntry idle => new IdleLogEntrySnapshot(
                idle.Id,
                idle.SessionId,
                idle.ActorId.Value,
                idle.ActorName,
                idle.EntryType.ToString(),
                idle.Info,
                idle.TurnIndex,
                idle.CreatedAt),
            ItemPickedUpLogEntry item => new ItemPickedUpLogEntrySnapshot(
                item.Id,
                item.SessionId,
                item.ActorId.Value,
                item.ActorName,
                item.EntryType.ToString(),
                item.Info,
                item.TurnIndex,
                item.CreatedAt,
                item.PlacedItemId,
                item.ItemId,
                item.ItemName,
                item.ItemType.ToString(),
                new PositionSnapshot(item.Position.X, item.Position.Y),
                item.Value,
                new StatSnapshotContract(item.ActorHp.Current, item.ActorHp.Max)),
            _ => throw new ArgumentOutOfRangeException(nameof(logEntry), logEntry.GetType().Name, null)
        };
}
