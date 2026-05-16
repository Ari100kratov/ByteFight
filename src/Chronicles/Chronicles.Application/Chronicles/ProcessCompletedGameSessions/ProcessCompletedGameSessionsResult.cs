namespace Chronicles.Application.Chronicles.ProcessCompletedGameSessions;

/// <summary>
/// Результат обработки очередной порции игровых сессий.
/// </summary>
public sealed record ProcessCompletedGameSessionsResult(
    int ProcessedSessions,
    int UpdatedCharacters,
    int CreatedRecords);
