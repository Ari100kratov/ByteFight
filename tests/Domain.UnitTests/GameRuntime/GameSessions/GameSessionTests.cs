using Domain.Game.GameModes;
using Domain.GameRuntime.GameResults;
using Domain.GameRuntime.GameSessions;
using Domain.GameRuntime.GameSessionParticipants;
using SharedKernel;
using Shouldly;
using Xunit;

namespace Domain.UnitTests.GameRuntime.GameSessions;

public sealed class GameSessionTests
{
    private static readonly DateTime Now = new(2026, 5, 16, 10, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void New_ShouldCreatePendingSessionWithPlayerAndNpcs()
    {
        var sessionId = Guid.CreateVersion7();
        var arenaId = Guid.CreateVersion7();
        var characterId = Guid.CreateVersion7();
        var userId = Guid.CreateVersion7();
        var enemyId = Guid.CreateVersion7();

        var session = GameSession.New(
            sessionId,
            GameModeType.Training,
            arenaId,
            characterId,
            "  Герой с очень длинным именем, которое должно быть сохранено полностью  ",
            userId,
            "  Иван  ",
            "Петров",
            "Воин",
            "Танк",
            [(enemyId, "Манекен")],
            new FixedDateTimeProvider(Now));

        session.Id.ShouldBe(sessionId);
        session.Status.ShouldBe(GameStatus.Pending);
        session.StartedAt.ShouldBe(Now);
        session.Participants.Count.ShouldBe(2);

        GameSessionParticipant player = session.Participants.Single(x => x.UnitType == ParticipantUnitType.Player);
        player.UnitId.Value.ShouldBe(characterId);
        player.UserId?.Value.ShouldBe(userId);
        player.UnitName.ShouldBe("Герой с очень длинным именем, которое должно быть сохранено полностью");
        player.UserFirstName.ShouldBe("Иван");

        GameSessionParticipant npc = session.Participants.Single(x => x.UnitType == ParticipantUnitType.Npc);
        npc.UnitId.Value.ShouldBe(enemyId);
        npc.UserId.ShouldBeNull();
    }

    [Fact]
    public void CompleteSuccess_ShouldRejectWinnerOutsideSession()
    {
        GameSession session = CreateSession();

        DomainException exception = Should.Throw<DomainException>(
            () => session.CompleteSuccess(
                GameResult.PlayerVictory(Guid.CreateVersion7()),
                turns: 3,
                new FixedDateTimeProvider(Now)));

        exception.Code.ShouldBe("INVALID_WINNER_UNIT");
        session.Status.ShouldBe(GameStatus.Pending);
    }

    [Fact]
    public void CompleteSuccess_ShouldBeIdempotentAfterSessionIsOver()
    {
        GameSession session = CreateSession();
        Guid playerUnitId = session.Participants.Single(x => x.UnitType == ParticipantUnitType.Player).UnitId.Value;

        session.CompleteSuccess(GameResult.PlayerVictory(playerUnitId), turns: 3, new FixedDateTimeProvider(Now));
        session.CompleteSuccess(GameResult.Draw(), turns: 99, new FixedDateTimeProvider(Now.AddMinutes(1)));

        session.Status.ShouldBe(GameStatus.Completed);
        session.TotalTurns.ShouldBe(3);
        session.Result?.Outcome.ShouldBe(GameOutcome.Victory);
        session.EndedAt.ShouldBe(Now);
    }

    [Fact]
    public void Fail_ShouldTruncateLongReasonAndCloseSession()
    {
        GameSession session = CreateSession();
        string reason = new('x', 300);

        session.Fail(reason, turns: 5, new FixedDateTimeProvider(Now));

        session.Status.ShouldBe(GameStatus.Failed);
        session.ErrorMessage?.Length.ShouldBe(256);
        session.TotalTurns.ShouldBe(5);
        session.EndedAt.ShouldBe(Now);
    }

    private static GameSession CreateSession() =>
        GameSession.New(
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
            [(Guid.CreateVersion7(), "Манекен")],
            new FixedDateTimeProvider(Now));

    private sealed class FixedDateTimeProvider(DateTime utcNow) : IDateTimeProvider
    {
        public DateTime UtcNow { get; } = utcNow;
    }
}
