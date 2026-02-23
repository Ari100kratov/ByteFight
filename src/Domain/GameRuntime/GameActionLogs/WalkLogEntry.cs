using Domain.Game.Actions;
using Domain.ValueObjects;

namespace Domain.GameRuntime.GameActionLogs;

public sealed class WalkLogEntry : GameActionLogEntry
{
    public FacingDirection FacingDirection { get; private set; }
    public Position To { get; private set; }

    private WalkLogEntry() { } // EF

    public WalkLogEntry(
        Guid sessionId,
        Guid actorId,
        string? info,
        FacingDirection facingDirection,
        Position to,
        int turnIndex)
        : base(sessionId, actorId, ActionType.Walk, info, turnIndex)
    {
        FacingDirection = facingDirection;
        To = to;
    }
}
