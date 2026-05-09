namespace Domain.GameRuntime.GameActionLogs;

/// <summary>
/// Запись журнала о смерти юнита.
/// </summary>
public sealed class DeathLogEntry : GameActionLogEntry
{
    private DeathLogEntry() { } // EF

    public DeathLogEntry(
        Guid sessionId,
        UnitId actorId,
        string actorName,
        string? info,
        int turnIndex)
        : base(
            sessionId,
            actorId,
            actorName,
            GameActionLogEntryType.Death,
            info,
            turnIndex)
    {
    }
}
