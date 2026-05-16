using Chronicles.Application.Abstractions.Data;
using Chronicles.Application.Chronicles.ProcessCompletedGameSessions;
using Microsoft.EntityFrameworkCore;

namespace Chronicles.Application.Chronicles.RebuildProjections;

internal sealed class ChroniclesReprojectionService(
    IChroniclesDbContext chroniclesDbContext,
    ICompletedGameSessionPayloadParser payloadParser,
    ChroniclesProjectionSessionProcessor sessionProcessor)
    : IChroniclesReprojectionService
{
    public async Task RebuildAsync(CancellationToken cancellationToken = default)
    {
        await chroniclesDbContext.CharacterNominationScores.ExecuteDeleteAsync(cancellationToken);
        await chroniclesDbContext.ChronicleRecords.ExecuteDeleteAsync(cancellationToken);
        await chroniclesDbContext.CharacterChronicleStats.ExecuteDeleteAsync(cancellationToken);

        List<string> processedPayloads = await chroniclesDbContext.InboxMessages
            .AsNoTracking()
            .Where(x => x.ProcessedAtUtc != null && x.DeadLetteredAtUtc == null)
            .OrderBy(x => x.ReceivedAtUtc)
            .Select(x => x.Payload)
            .ToListAsync(cancellationToken);

        foreach (string payload in processedPayloads)
        {
            CompletedGameSessionData session = payloadParser.Parse(payload);
            await sessionProcessor.ApplyAsync(session, cancellationToken);
        }

        await chroniclesDbContext.SaveChangesAsync(cancellationToken);
    }
}
