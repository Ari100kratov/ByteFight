using Domain.Game.GameModes;
using Domain.GameRuntime.GameActionLogs.Entries;
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
        string characterName,
        Guid userId,
        string? userFirstName,
        string? userLastName,
        string? characterClassName,
        string? characterSpecName,
        IEnumerable<(Guid ArenaEnemyId, string Name)> arenaEnemies,
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

        gameSession.AddPlayer(
            characterId,
            characterName,
            userId,
            userFirstName,
            userLastName,
            characterClassName,
            characterSpecName,
            dateTimeProvider.UtcNow);

        foreach ((Guid arenaEnemyId, string name) in arenaEnemies)
        {
            gameSession.AddNpc(arenaEnemyId, name, dateTimeProvider.UtcNow);
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
        ErrorMessage = reason?.Length > 256 ? reason[..256] : reason;
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

    private void AddPlayer(
        Guid characterId,
        string characterName,
        Guid userId,
        string? userFirstName,
        string? userLastName,
        string? characterClassName,
        string? characterSpecName,
        DateTime joinedAt) =>
        AddParticipant(
            ParticipantUnitType.Player,
            new UnitId(characterId),
            characterName,
            new UserId(userId),
            userFirstName,
            userLastName,
            characterClassName,
            characterSpecName,
            joinedAt);

    private void AddNpc(Guid arenaEnemyId, string name, DateTime joinedAt) =>
        AddParticipant(ParticipantUnitType.Npc, new UnitId(arenaEnemyId), name, null, null, null, null, null, joinedAt);

    private void AddParticipant(
        ParticipantUnitType unitType,
        UnitId unitId,
        string unitName,
        UserId? userId,
        string? userFirstName,
        string? userLastName,
        string? characterClassName,
        string? characterSpecName,
        DateTime joinedAt)
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
            UnitName = Trim(unitName, 128) ?? string.Empty,
            UserId = userId,
            UserFirstName = Trim(userFirstName, 100),
            UserLastName = Trim(userLastName, 100),
            CharacterClassName = Trim(characterClassName, 128),
            CharacterSpecName = Trim(characterSpecName, 128),
            JoinedAt = joinedAt
        };

        _participants.Add(participant);
    }

    private static string? Trim(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        string trimmed = value.Trim();

        return trimmed.Length > maxLength
            ? trimmed[..maxLength]
            : trimmed;
    }
}
