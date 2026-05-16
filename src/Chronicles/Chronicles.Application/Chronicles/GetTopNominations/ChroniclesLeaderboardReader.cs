using Chronicles.Application.Abstractions.Data;
using Chronicles.Domain;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Chronicles.Application.Chronicles.GetTopNominations;

internal sealed class ChroniclesLeaderboardReader(IChroniclesDbContext chroniclesDbContext) : IChroniclesLeaderboardReader
{
    public async Task<Result<IReadOnlyList<ChronicleNominationLeaderboardResponse>>> GetTopAsync(
        int top,
        CancellationToken cancellationToken)
    {
        List<ChronicleNomination> nominations = await chroniclesDbContext.ChronicleNominations
            .AsNoTracking()
            .OrderBy(x => x.Type)
            .ToListAsync(cancellationToken);

        List<ChronicleNominationLeaderboardResponse> leaderboards = new(nominations.Count);

        foreach (ChronicleNomination nomination in nominations)
        {
            List<CharacterNominationScore> scores = await GetScoresAsync(nomination, top, cancellationToken);

            var entries = scores
                .Select((score, index) => new ChronicleNominationEntryResponse(
                    index + 1,
                    score.CharacterId,
                    score.CharacterName,
                    score.UserFirstName,
                    score.UserLastName,
                    score.CharacterClassName,
                    score.CharacterSpecName,
                    score.Value,
                    score.SessionId,
                    score.OccurredAtUtc))
                .ToList();

            leaderboards.Add(new ChronicleNominationLeaderboardResponse(
                nomination.Code,
                nomination.Title,
                nomination.Description,
                nomination.MetricLabel,
                nomination.MetricUnit,
                ToContract(nomination.ValueKind),
                ToContract(nomination.SortDirection),
                entries));
        }

        return Result.Success<IReadOnlyList<ChronicleNominationLeaderboardResponse>>(leaderboards);
    }

    private async Task<List<CharacterNominationScore>> GetScoresAsync(
        ChronicleNomination nomination,
        int top,
        CancellationToken cancellationToken)
    {
        IQueryable<CharacterNominationScore> query = chroniclesDbContext.CharacterNominationScores
            .AsNoTracking()
            .Where(x => x.NominationType == nomination.Type);

        query = nomination.SortDirection == ChronicleNominationSortDirection.Ascending
            ? query.OrderBy(x => x.Value).ThenBy(x => x.OccurredAtUtc).ThenBy(x => x.CharacterName)
            : query.OrderByDescending(x => x.Value).ThenBy(x => x.OccurredAtUtc).ThenBy(x => x.CharacterName);

        return await query
            .Take(top)
            .ToListAsync(cancellationToken);
    }

    private static string ToContract(ChronicleNominationValueKind valueKind) =>
        valueKind switch
        {
            ChronicleNominationValueKind.Turns => "turns",
            ChronicleNominationValueKind.Count => "count",
            ChronicleNominationValueKind.Damage => "damage",
            ChronicleNominationValueKind.Healing => "healing",
            _ => throw new InvalidOperationException($"Unsupported nomination value kind '{valueKind}'.")
        };

    private static string ToContract(ChronicleNominationSortDirection sortDirection) =>
        sortDirection switch
        {
            ChronicleNominationSortDirection.Ascending => "ascending",
            ChronicleNominationSortDirection.Descending => "descending",
            _ => throw new InvalidOperationException($"Unsupported nomination sort direction '{sortDirection}'.")
        };
}
