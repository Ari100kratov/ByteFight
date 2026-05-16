using Microsoft.Extensions.Logging;

namespace Chronicles.Application.Chronicles.ProcessCompletedGameSessions;

internal static partial class ChroniclesProjectionLogMessages
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "Обработано игровых сессий: {ProcessedSessions}. Обновлено персонажей: {UpdatedCharacters}. Создано записей хроник: {CreatedRecords}.")]
    public static partial void BatchProcessed(
        ILogger logger,
        int processedSessions,
        int updatedCharacters,
        int createdRecords);
}
