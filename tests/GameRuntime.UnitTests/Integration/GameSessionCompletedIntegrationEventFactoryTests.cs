using Domain.Game.Abilities;
using Domain.Game.GameModes;
using Domain.GameRuntime.GameActionLogs;
using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.GameRuntime.GameResults;
using Domain.GameRuntime.GameSessions;
using Domain.GameRuntime.GameSessionParticipants;
using Domain.ValueObjects;
using GameRuntime.Integration;
using IntegrationContracts.GameSessions;
using SharedKernel;
using Shouldly;
using Xunit;

namespace GameRuntime.UnitTests.Integration;

public sealed class GameSessionCompletedIntegrationEventFactoryTests
{
    private static readonly DateTime StartedAtUtc = new(2026, 5, 16, 10, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime EndedAtUtc = StartedAtUtc.AddMinutes(3);
    private static readonly DateTime CreatedAtUtc = EndedAtUtc.AddSeconds(1);

    [Fact]
    public void Create_ShouldMapCompletedSessionParticipantsAndLogs()
    {
        GameSession session = CreateCompletedSession();
        GameSessionParticipant player = session.Participants.Single(x => x.UnitType == ParticipantUnitType.Player);
        GameSessionParticipant npc = session.Participants.Single(x => x.UnitType == ParticipantUnitType.Npc);
        var log = new AbilityUsedLogEntry(
            session.Id,
            player.UnitId,
            player.UnitName,
            null,
            AbilityType.BasicMeleeAttack,
            AbilityEffectType.Damage,
            "Удар",
            npc.UnitId,
            npc.UnitName,
            12,
            FacingDirection.Right,
            new StatSnapshot(0, 12),
            turnIndex: 1);

        var factory = new GameSessionCompletedIntegrationEventFactory(new FixedDateTimeProvider(CreatedAtUtc));

        GameSessionCompletedIntegrationEvent integrationEvent = factory.Create(session, [log]);

        integrationEvent.SchemaVersion.ShouldBe(1);
        integrationEvent.SessionId.ShouldBe(session.Id);
        integrationEvent.CreatedAtUtc.ShouldBe(CreatedAtUtc);
        integrationEvent.ArenaId.ShouldBe(session.ArenaId.Value);
        integrationEvent.StartedAtUtc.ShouldBe(StartedAtUtc);
        integrationEvent.EndedAtUtc.ShouldBe(EndedAtUtc);
        integrationEvent.TotalTurns.ShouldBe(4);
        integrationEvent.Outcome.ShouldBe(IntegrationGameOutcome.Victory);
        integrationEvent.WinnerUnitId.ShouldBe(player.UnitId.Value);

        GameSessionParticipantSnapshot participant = integrationEvent.Participants.Single(x => x.UnitType == IntegrationParticipantUnitType.Player);
        participant.UserId.ShouldBe(player.UserId?.Value);
        participant.UserFirstName.ShouldBe("Иван");
        participant.UserLastName.ShouldBe("Петров");
        participant.CharacterClassName.ShouldBe("Воин");
        participant.CharacterSpecName.ShouldBe("Танк");

        AbilityUsedLogEntrySnapshot mappedLog = integrationEvent.Logs.Single().ShouldBeOfType<AbilityUsedLogEntrySnapshot>();
        mappedLog.ActorId.ShouldBe(player.UnitId.Value);
        mappedLog.TargetId.ShouldBe(npc.UnitId.Value);
        mappedLog.EffectType.ShouldBe(nameof(AbilityEffectType.Damage));
        mappedLog.TargetHp.Current.ShouldBe(0);
    }

    [Fact]
    public void Create_ShouldRejectSessionWithoutCompletionMetadata()
    {
        var pendingSession = GameSession.New(
            Guid.CreateVersion7(),
            GameModeType.Training,
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            "Герой",
            Guid.CreateVersion7(),
            "Иван",
            "Петров",
            "Воин",
            "Танк",
            [],
            new FixedDateTimeProvider(StartedAtUtc));

        var factory = new GameSessionCompletedIntegrationEventFactory(new FixedDateTimeProvider(CreatedAtUtc));

        Should.Throw<InvalidOperationException>(() => factory.Create(pendingSession, []));
    }

    private static GameSession CreateCompletedSession()
    {
        var characterId = Guid.CreateVersion7();
        var session = GameSession.New(
            Guid.CreateVersion7(),
            GameModeType.Training,
            Guid.CreateVersion7(),
            characterId,
            "Герой",
            Guid.CreateVersion7(),
            "Иван",
            "Петров",
            "Воин",
            "Танк",
            [(Guid.CreateVersion7(), "Манекен")],
            new FixedDateTimeProvider(StartedAtUtc));

        session.CompleteSuccess(
            GameResult.PlayerVictory(characterId),
            turns: 4,
            new FixedDateTimeProvider(EndedAtUtc));

        return session;
    }

    private sealed class FixedDateTimeProvider(DateTime utcNow) : IDateTimeProvider
    {
        public DateTime UtcNow { get; } = utcNow;
    }
}
