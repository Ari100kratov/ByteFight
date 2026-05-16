using Chronicles.Application.Abstractions.Data;
using Chronicles.Domain;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Chronicles.Application.Chronicles.GetCharacterChronicles;

internal sealed class CharacterChroniclesReader(IChroniclesDbContext chroniclesDbContext) : ICharacterChroniclesReader
{
    public async Task<Result<CharacterChroniclesResponse>> GetAsync(Guid characterId, CancellationToken cancellationToken)
    {
        CharacterChronicleStats? stats = await chroniclesDbContext.CharacterChronicleStats
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.CharacterId == characterId, cancellationToken);

        if (stats is null)
        {
            return Result.Failure<CharacterChroniclesResponse>(
                Error.NotFound("Chronicles.Character.NotFound", $"Персонаж '{characterId}' ещё не имеет записей в хрониках."));
        }

        Dictionary<ChronicleNominationType, string> nominationCodes = await chroniclesDbContext.ChronicleNominations
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Type, x => x.Code, cancellationToken);

        List<ChronicleRecord> records = await chroniclesDbContext.ChronicleRecords
            .AsNoTracking()
            .Where(x => x.CharacterId == characterId)
            .OrderByDescending(x => x.OccurredAtUtc)
            .Take(10)
            .ToListAsync(cancellationToken);

        var recentRecords = records
            .Select(x => new CharacterChronicleRecordResponse(
                nominationCodes[x.NominationType],
                x.UserFirstName,
                x.UserLastName,
                x.CharacterClassName,
                x.CharacterSpecName,
                x.Value,
                x.OccurredAtUtc))
            .ToList();

        return Result.Success(new CharacterChroniclesResponse(
            stats.CharacterId,
            stats.CharacterName,
            stats.BattlesPlayed,
            stats.Victories,
            stats.Defeats,
            stats.Draws,
            stats.TotalTurnsPlayed,
            stats.LastSessionAtUtc,
            recentRecords));
    }
}
