using Domain.ValueObjects;

namespace Domain.GameRuntime.GameActionLogs;

/// <summary>
/// Запись журнала о перемещении юнита.
/// </summary>
public sealed class WalkLogEntry : GameActionLogEntry
{
    public FacingDirection FacingDirection { get; private set; }
    public Position To { get; private set; }

    private WalkLogEntry() { } // EF

    public WalkLogEntry(
        Guid sessionId,
        UnitId actorId,
        string actorName,
        string? info,
        FacingDirection facingDirection,
        Position to,
        int turnIndex)
        : base(
            sessionId,
            actorId,
            actorName,
            GameActionLogEntryType.Walk,
            info,
            turnIndex)
    {
        FacingDirection = facingDirection;
        To = to;
    }
}
