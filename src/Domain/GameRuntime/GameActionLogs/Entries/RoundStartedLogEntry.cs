namespace Domain.GameRuntime.GameActionLogs.Entries;

/// <summary>
/// Запись журнала о начале раунда боя.
/// </summary>
public sealed class RoundStartedLogEntry : GameActionLogEntry
{
    /// <summary>
    /// Номер начавшегося раунда.
    /// </summary>
    public int RoundNumber { get; private set; }

    private RoundStartedLogEntry() { } // EF

    public RoundStartedLogEntry(
        Guid sessionId,
        UnitId actorId,
        string actorName,
        string? info,
        int roundNumber,
        int turnIndex)
        : base(
            sessionId,
            actorId,
            actorName,
            GameActionLogEntryType.RoundStarted,
            info,
            turnIndex)
    {
        RoundNumber = roundNumber;
    }
}
