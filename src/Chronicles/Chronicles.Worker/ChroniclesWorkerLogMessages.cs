namespace Chronicles.Worker;

internal static partial class ChroniclesWorkerLogMessages
{
    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Information,
        Message = "Фоновая обработка хроник завершила пакет. Импортировано сообщений: {ImportedMessages}. Обработано игровых сессий: {ProcessedSessions}.")]
    public static partial void BatchCompleted(ILogger logger, int importedMessages, int processedSessions);

    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Error,
        Message = "Фоновая обработка хроник завершилась ошибкой.")]
    public static partial void BatchFailed(ILogger logger, Exception exception);
}
