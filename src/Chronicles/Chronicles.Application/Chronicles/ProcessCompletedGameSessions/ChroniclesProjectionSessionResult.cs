namespace Chronicles.Application.Chronicles.ProcessCompletedGameSessions;

internal sealed record ChroniclesProjectionSessionResult(
    int UpdatedCharacters,
    int CreatedRecords,
    int UpdatedScores);
