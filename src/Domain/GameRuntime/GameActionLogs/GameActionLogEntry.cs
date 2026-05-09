using SharedKernel;

namespace Domain.GameRuntime.GameActionLogs;

/// <summary>
/// Базовая запись журнала боя.
/// </summary>
public abstract class GameActionLogEntry : Entity
{
    public Guid Id { get; protected set; }
    public Guid SessionId { get; protected set; }

    public UnitId ActorId { get; protected set; }
    public string ActorName { get; protected set; }

    /// <summary>
    /// Тип записи журнала.
    /// </summary>
    public GameActionLogEntryType EntryType { get; protected set; }

    public string? Info { get; protected set; }

    public int TurnIndex { get; protected set; }
    public DateTime CreatedAt { get; protected set; }

    protected GameActionLogEntry()
    {
    }

    protected GameActionLogEntry(
        Guid sessionId,
        UnitId actorId,
        string actorName,
        GameActionLogEntryType entryType,
        string? info,
        int turnIndex)
    {
        Id = Guid.CreateVersion7();
        SessionId = sessionId;
        ActorId = actorId;
        ActorName = actorName;
        EntryType = entryType;
        Info = info?.Length > 256 ? info[..256] : info;
        TurnIndex = turnIndex;
        CreatedAt = DateTime.UtcNow;
    }
}
