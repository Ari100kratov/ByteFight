using Domain.Game.Actions;
using SharedKernel;

namespace Domain.GameRuntime.GameActionLogs;

public abstract class GameActionLogEntry : Entity
{
    public Guid Id { get; protected set; }
    public Guid SessionId { get; protected set; }

    public Guid ActorId { get; protected set; }
    public ActionType ActionType { get; protected set; }
    public string? Info { get; protected set; }

    public int TurnIndex { get; protected set; }
    public DateTime CreatedAt { get; protected set; }

    protected GameActionLogEntry() { } // EF

    protected GameActionLogEntry(
        Guid sessionId,
        Guid actorId,
        ActionType actionType,
        string? info,
        int turnIndex)
    {
        Id = Guid.CreateVersion7();
        SessionId = sessionId;
        ActorId = actorId;
        ActionType = actionType;
        Info = info;
        TurnIndex = turnIndex;
        CreatedAt = DateTime.UtcNow;
    }
}
