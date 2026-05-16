namespace Chronicles.Application.Abstractions.Data;

/// <summary>
/// Завершённая игровая сессия в проекции, пригодной для расчёта хроник.
/// </summary>
public sealed record CompletedGameSessionData(
    Guid SessionId,
    int TotalTurns,
    DateTime StartedAtUtc,
    DateTime EndedAtUtc,
    CompletedGameSessionOutcome Outcome,
    Guid? WinnerUnitId,
    IReadOnlyList<CompletedGameSessionParticipantData> Participants,
    IReadOnlyList<CompletedGameSessionLogEntryData> Logs);
