namespace Domain.GameRuntime.GameActionLogs;

public sealed record TurnLog
{
    public required int TurnIndex { get; init; }
    public required IReadOnlyList<GameActionLogEntry> Logs { get; init; }
}
