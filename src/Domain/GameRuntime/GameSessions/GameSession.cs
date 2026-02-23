using Domain.Game.GameModes;
using Domain.GameRuntime.GameActionLogs;
using Domain.GameRuntime.GameResults;
using Domain.GameRuntime.GameSessionParticipants;
using SharedKernel;

namespace Domain.GameRuntime.GameSessions;

public class GameSession : Entity
{
    public Guid Id { get; set; }

    public GameModeType Mode { get; set; }
    public ArenaId ArenaId { get; set; }
    public List<Guid> UserIds { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }

    public int TotalTurns { get; set; }
    public GameStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public GameResult? Result { get; set; }

    private readonly List<GameSessionParticipant> _participants = [];
    public IReadOnlyCollection<GameSessionParticipant> Participants => _participants.AsReadOnly();

    private readonly List<GameActionLogEntry> _actionLogs = [];
    public IReadOnlyCollection<GameActionLogEntry> ActionLogs => _actionLogs.AsReadOnly();


    public bool IsOver => Status is GameStatus.Completed or GameStatus.Failed or GameStatus.Aborted;

    public static GameSession New(
        Guid id,
        GameModeType mode,
        Guid arenaId,
        Guid characterId,
        Guid userId,
        IEnumerable<Guid> arenaEnemyIds,
        IDateTimeProvider dateTimeProvider)
    {
        var gameSession = new GameSession
        {
            Id = id,
            Mode = mode,
            ArenaId = new ArenaId(arenaId),
            UserIds = [userId],
            StartedAt = dateTimeProvider.UtcNow,
            Status = GameStatus.Pending,
        };

        gameSession.AddPlayer(characterId, userId, dateTimeProvider.UtcNow);

        foreach (Guid arenaEnemyId in arenaEnemyIds)
        {
            gameSession.AddNpc(arenaEnemyId, dateTimeProvider.UtcNow);
        }

        return gameSession;
    }

    public void Start(IDateTimeProvider dateTimeProvider)
    {
        if (Status is not GameStatus.Pending)
        {
            throw new DomainException("SESSION_INVALID_STATE",
                "Cannot start session that is not pending.");
        }

        Status = GameStatus.Active;
    }

    public void CompleteSuccess(GameResult gameResult, int turns, IDateTimeProvider dateTimeProvider)
    {
        if (IsOver)
        {
            return;
        }

        if (gameResult.WinnerUnitId is not null)
        {
            bool exists = _participants.Any(p => p.UnitId == gameResult.WinnerUnitId);
            if (!exists)
            {
                throw new DomainException(
                    "INVALID_WINNER_UNIT",
                    $"WinnerUnitId '{gameResult.WinnerUnitId}' does not exist in the session participants."
                );
            }
        }

        Result = gameResult;
        Status = GameStatus.Completed;
        TotalTurns = turns;
        EndedAt = dateTimeProvider.UtcNow;
    }

    public void Fail(string? reason, int turns, IDateTimeProvider dateTimeProvider)
    {
        if (IsOver)
        {
            return;
        }

        Status = GameStatus.Failed;
        ErrorMessage = reason;
        TotalTurns = turns;
        EndedAt = dateTimeProvider.UtcNow;
    }

    public void Abort(int turns, IDateTimeProvider dateTimeProvider)
    {
        if (IsOver)
        {
            return;
        }

        Status = GameStatus.Aborted;
        TotalTurns = turns;
        EndedAt = dateTimeProvider.UtcNow;
    }

    private void AddPlayer(Guid characterId, Guid userId, DateTime joinedAt) =>
        AddParticipant(ParticipantUnitType.Player, new UnitId(characterId), new UserId(userId), joinedAt);

    private void AddNpc(Guid arenaEnemyId, DateTime joinedAt) =>
        AddParticipant(ParticipantUnitType.Npc, new UnitId(arenaEnemyId), null, joinedAt);

    private void AddParticipant(ParticipantUnitType unitType, UnitId unitId, UserId? userId, DateTime joinedAt)
    {
        if (IsOver)
        {
            throw new DomainException("SESSION_NOT_ACTIVE",
                "Cannot modify participants for a session that is no longer active.");
        }

        var participant = new GameSessionParticipant
        {
            Id = Guid.CreateVersion7(),
            SessionId = Id,
            UnitType = unitType,
            UnitId = unitId,
            UserId = userId,
            JoinedAt = joinedAt
        };

        _participants.Add(participant);
    }
}
