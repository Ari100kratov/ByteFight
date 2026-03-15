using Domain.Game.Actions;

namespace Domain.GameRuntime.GameActionLogs;

public sealed class DeathLogEntry : GameActionLogEntry
{
    private DeathLogEntry() { } // EF

    public DeathLogEntry(
        Guid sessionId,
        UnitId actorId,
        string actorName,
        string? info,
        int turnIndex)
        : base(sessionId, actorId, actorName, ActionType.Dead, info, turnIndex)
    {
    }
}
