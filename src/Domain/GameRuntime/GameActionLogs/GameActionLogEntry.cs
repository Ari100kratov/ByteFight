using Domain.Game.Actions;
using SharedKernel;

namespace Domain.GameRuntime.GameActionLogs;

public abstract class GameActionLogEntry : Entity
{
    public Guid Id { get; protected set; }
    public Guid SessionId { get; protected set; }

    public UnitId ActorId { get; protected set; }
    public string ActorName { get; protected set; }
    public ActionType ActionType { get; protected set; }
    public string? Info { get; protected set; }

    public int TurnIndex { get; protected set; }
    public DateTime CreatedAt { get; protected set; }

    protected GameActionLogEntry() { } // EF

    protected GameActionLogEntry(
        Guid sessionId,
        UnitId actorId,
        string actorName,
        ActionType actionType,
        string? info,
        int turnIndex)
    {
        Id = Guid.CreateVersion7();
        SessionId = sessionId;
        ActorId = actorId;
        ActorName = actorName;
        ActionType = actionType;
        Info = info;
        TurnIndex = turnIndex;
        CreatedAt = DateTime.UtcNow;
    }
}
