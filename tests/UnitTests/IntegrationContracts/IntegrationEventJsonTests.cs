using System.Text.Json;
using IntegrationContracts;
using IntegrationContracts.GameSessions;
using Shouldly;
using Xunit;

namespace UnitTests.IntegrationContracts;

public sealed class IntegrationEventJsonTests
{
    [Fact]
    public void SerializerOptions_ShouldWritePolymorphicLogEntryDiscriminator()
    {
        GameSessionCompletedIntegrationEvent integrationEvent = CreateCompletedSessionEvent();

        string json = JsonSerializer.Serialize(
            integrationEvent,
            IntegrationEventJson.SerializerOptions);

        json.ShouldContain("\"kind\":\"idle\"");
    }

    [Fact]
    public void SerializerOptions_ShouldReadPolymorphicLogEntryWhenDiscriminatorIsNotFirstProperty()
    {
        var eventId = Guid.CreateVersion7();
        var sessionId = Guid.CreateVersion7();
        var arenaId = Guid.CreateVersion7();
        var logId = Guid.CreateVersion7();
        var actorId = Guid.CreateVersion7();

        string json = $$"""
            {
              "schemaVersion": 1,
              "eventId": "{{eventId}}",
              "sessionId": "{{sessionId}}",
              "createdAtUtc": "2026-05-16T10:00:00Z",
              "arenaId": "{{arenaId}}",
              "startedAtUtc": "2026-05-16T09:55:00Z",
              "endedAtUtc": "2026-05-16T10:00:00Z",
              "totalTurns": 1,
              "outcome": 1,
              "winnerUnitId": "{{actorId}}",
              "participants": [],
              "logs": [
                {
                  "id": "{{logId}}",
                  "kind": "idle",
                  "sessionId": "{{sessionId}}",
                  "actorId": "{{actorId}}",
                  "actorName": "Тестовый герой",
                  "entryType": "Idle",
                  "info": null,
                  "turnIndex": 0,
                  "createdAtUtc": "2026-05-16T09:56:00Z"
                }
              ]
            }
            """;

        GameSessionCompletedIntegrationEvent? integrationEvent = JsonSerializer.Deserialize<GameSessionCompletedIntegrationEvent>(
            json,
            IntegrationEventJson.SerializerOptions);

        integrationEvent.ShouldNotBeNull();
        integrationEvent.Logs.Single().ShouldBeOfType<IdleLogEntrySnapshot>();
    }

    private static GameSessionCompletedIntegrationEvent CreateCompletedSessionEvent()
    {
        var sessionId = Guid.CreateVersion7();
        var actorId = Guid.CreateVersion7();

        return new GameSessionCompletedIntegrationEvent
        {
            SchemaVersion = 1,
            EventId = Guid.CreateVersion7(),
            SessionId = sessionId,
            CreatedAtUtc = new DateTime(2026, 5, 16, 10, 0, 0, DateTimeKind.Utc),
            ArenaId = Guid.CreateVersion7(),
            StartedAtUtc = new DateTime(2026, 5, 16, 9, 55, 0, DateTimeKind.Utc),
            EndedAtUtc = new DateTime(2026, 5, 16, 10, 0, 0, DateTimeKind.Utc),
            TotalTurns = 1,
            Outcome = IntegrationGameOutcome.Victory,
            WinnerUnitId = actorId,
            Participants = [],
            Logs =
            [
                new IdleLogEntrySnapshot(
                    Guid.CreateVersion7(),
                    sessionId,
                    actorId,
                    "Тестовый герой",
                    "Idle",
                    null,
                    0,
                    new DateTime(2026, 5, 16, 9, 56, 0, DateTimeKind.Utc))
            ]
        };
    }
}
