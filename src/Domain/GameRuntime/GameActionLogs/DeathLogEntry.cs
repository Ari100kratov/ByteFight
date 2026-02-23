using Domain.Game.Actions;

namespace Domain.GameRuntime.GameActionLogs;

public sealed class DeathLogEntry : GameActionLogEntry
{
    private DeathLogEntry() { } // EF

    public DeathLogEntry(
        Guid sessionId,
        Guid actorId,
        string? info,
        int turnIndex)
        : base(sessionId, actorId, ActionType.Dead, info, turnIndex)
    {
    }
}
