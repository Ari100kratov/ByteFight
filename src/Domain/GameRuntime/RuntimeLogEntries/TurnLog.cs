namespace Domain.GameRuntime.RuntimeLogEntries;

public sealed record TurnLog
{
    public required int TurnIndex { get; init; }
    public required IReadOnlyList<RuntimeLogEntry> Logs { get; init; }
}
