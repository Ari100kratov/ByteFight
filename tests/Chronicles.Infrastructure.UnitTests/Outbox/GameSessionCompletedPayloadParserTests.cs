using System.Text.Json;
using Chronicles.Application.Abstractions.Data;
using Chronicles.Infrastructure.Outbox;
using IntegrationContracts;
using IntegrationContracts.GameSessions;
using Shouldly;
using Xunit;

namespace Chronicles.Infrastructure.UnitTests.Outbox;

/// <summary>
/// Проверяет разбор JSON payload интеграционного события завершённой игровой сессии.
/// </summary>
public sealed class GameSessionCompletedPayloadParserTests
{
    [Fact]
    public void Parse_ShouldMapIntegrationEventToCompletedGameSessionData()
    {
        GameSessionCompletedIntegrationEvent integrationEvent = CreateIntegrationEvent();
        string payload = JsonSerializer.Serialize(integrationEvent, IntegrationEventJson.SerializerOptions);

        CompletedGameSessionData result = new GameSessionCompletedPayloadParser().Parse(payload);

        result.SessionId.ShouldBe(integrationEvent.SessionId);
        result.TotalTurns.ShouldBe(integrationEvent.TotalTurns);
        result.Outcome.ShouldBe(CompletedGameSessionOutcome.Victory);
        result.WinnerUnitId.ShouldBe(integrationEvent.WinnerUnitId);
        result.Participants.Count.ShouldBe(2);

        CompletedGameSessionParticipantData player = result.Participants.Single(x => x.UnitType == CompletedParticipantUnitType.Player);
        player.UserFirstName.ShouldBe("Иван");
        player.UserLastName.ShouldBe("Петров");
        player.CharacterClassName.ShouldBe("Воин");
        player.CharacterSpecName.ShouldBe("Берсерк");

        result.Logs.Count.ShouldBe(5);
        result.Logs.ShouldContain(x => x.Kind == CompletedGameSessionLogEntryKind.Idle);
        result.Logs.ShouldContain(x => x.Kind == CompletedGameSessionLogEntryKind.Walk);
        result.Logs.ShouldContain(x => x.Kind == CompletedGameSessionLogEntryKind.Death);
        result.Logs.ShouldContain(x =>
            x.Kind == CompletedGameSessionLogEntryKind.AbilityUsed &&
            x.TargetId == integrationEvent.Participants[1].UnitId &&
            x.Value == 12 &&
            x.EffectType == "Damage");
        result.Logs.ShouldContain(x =>
            x.Kind == CompletedGameSessionLogEntryKind.ItemPickedUp &&
            x.Value == 5 &&
            x.ItemType == "HealingPotion");
    }

    [Fact]
    public void Parse_ShouldRejectUnsupportedSchemaVersion()
    {
        GameSessionCompletedIntegrationEvent integrationEvent = CreateIntegrationEvent() with
        {
            SchemaVersion = 2
        };
        string payload = JsonSerializer.Serialize(integrationEvent, IntegrationEventJson.SerializerOptions);

        InvalidOperationException exception = Should.Throw<InvalidOperationException>(
            () => new GameSessionCompletedPayloadParser().Parse(payload));

        exception.Message.ShouldContain("Unsupported integration event schema version");
    }

    private static GameSessionCompletedIntegrationEvent CreateIntegrationEvent()
    {
        var sessionId = Guid.CreateVersion7();
        var playerId = Guid.CreateVersion7();
        var npcId = Guid.CreateVersion7();
        DateTime startedAtUtc = new(2026, 5, 16, 10, 0, 0, DateTimeKind.Utc);
        DateTime endedAtUtc = startedAtUtc.AddMinutes(5);

        return new GameSessionCompletedIntegrationEvent
        {
            SchemaVersion = 1,
            EventId = Guid.CreateVersion7(),
            SessionId = sessionId,
            CreatedAtUtc = endedAtUtc,
            ArenaId = Guid.CreateVersion7(),
            StartedAtUtc = startedAtUtc,
            EndedAtUtc = endedAtUtc,
            TotalTurns = 6,
            Outcome = IntegrationGameOutcome.Victory,
            WinnerUnitId = playerId,
            Participants =
            [
                new GameSessionParticipantSnapshot(
                    playerId,
                    Guid.CreateVersion7(),
                    IntegrationParticipantUnitType.Player,
                    "Смелый воин",
                    "Иван",
                    "Петров",
                    "Воин",
                    "Берсерк"),
                new GameSessionParticipantSnapshot(
                    npcId,
                    null,
                    IntegrationParticipantUnitType.Npc,
                    "Манекен")
            ],
            Logs =
            [
                new IdleLogEntrySnapshot(
                    Guid.CreateVersion7(),
                    sessionId,
                    playerId,
                    "Смелый воин",
                    "Idle",
                    null,
                    1,
                    startedAtUtc.AddSeconds(10)),
                new WalkLogEntrySnapshot(
                    Guid.CreateVersion7(),
                    sessionId,
                    playerId,
                    "Смелый воин",
                    "Walk",
                    null,
                    2,
                    startedAtUtc.AddSeconds(20),
                    "Right",
                    new PositionSnapshot(2, 3)),
                new DeathLogEntrySnapshot(
                    Guid.CreateVersion7(),
                    sessionId,
                    npcId,
                    "Манекен",
                    "Death",
                    null,
                    3,
                    startedAtUtc.AddSeconds(30)),
                new AbilityUsedLogEntrySnapshot(
                    Guid.CreateVersion7(),
                    sessionId,
                    playerId,
                    "Смелый воин",
                    "AbilityUsed",
                    null,
                    4,
                    startedAtUtc.AddSeconds(40),
                    "Slash",
                    "Damage",
                    "Удар",
                    npcId,
                    "Манекен",
                    12,
                    "Right",
                    new StatSnapshotContract(0, 12)),
                new ItemPickedUpLogEntrySnapshot(
                    Guid.CreateVersion7(),
                    sessionId,
                    playerId,
                    "Смелый воин",
                    "ItemPickedUp",
                    null,
                    5,
                    startedAtUtc.AddSeconds(50),
                    Guid.CreateVersion7(),
                    Guid.CreateVersion7(),
                    "Зелье",
                    "HealingPotion",
                    new PositionSnapshot(1, 1),
                    5,
                    new StatSnapshotContract(10, 20))
            ]
        };
    }
}
