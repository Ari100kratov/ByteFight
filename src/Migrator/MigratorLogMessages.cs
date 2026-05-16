using Microsoft.Extensions.Logging;

namespace Migrator;

internal static partial class MigratorLogMessages
{
    [LoggerMessage(
        EventId = 3001,
        Level = LogLevel.Information,
        Message = "Backfilled completed game sessions into GameRuntime outbox. Sessions: {BackfilledSessions}.")]
    public static partial void CompletedSessionsBackfilled(ILogger logger, int backfilledSessions);

    [LoggerMessage(
        EventId = 3002,
        Level = LogLevel.Information,
        Message = "Caught up Chronicles projections. Imported messages: {ImportedMessages}. Processed sessions: {ProcessedSessions}.")]
    public static partial void ChroniclesCatchUpCompleted(
        ILogger logger,
        int importedMessages,
        int processedSessions);

    [LoggerMessage(
        EventId = 3003,
        Level = LogLevel.Information,
        Message = "Rebuilding Chronicles read models because explicit rebuild was requested.")]
    public static partial void ChroniclesRebuildRequested(ILogger logger);
}
