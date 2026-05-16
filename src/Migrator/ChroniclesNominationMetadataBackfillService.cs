using Chronicles.Domain;
using Chronicles.Infrastructure.Database;
using Infrastructure.Database.Auth;
using Infrastructure.Database.Game;
using Microsoft.EntityFrameworkCore;

namespace Migrator;

internal sealed class ChroniclesNominationMetadataBackfillService(
    ChroniclesDbContext chroniclesDbContext,
    GameDbContext gameDbContext,
    AuthDbContext authDbContext)
{
    public async Task BackfillAsync(CancellationToken cancellationToken = default)
    {
        List<CharacterNominationScore> scores = await chroniclesDbContext.CharacterNominationScores
            .Where(x =>
                x.UserFirstName == null ||
                x.UserFirstName == "" ||
                x.UserLastName == null ||
                x.UserLastName == "" ||
                x.CharacterClassName == null ||
                x.CharacterClassName == "" ||
                x.CharacterSpecName == null ||
                x.CharacterSpecName == "")
            .ToListAsync(cancellationToken);

        List<ChronicleRecord> records = await chroniclesDbContext.ChronicleRecords
            .Where(x =>
                x.UserFirstName == null ||
                x.UserFirstName == "" ||
                x.UserLastName == null ||
                x.UserLastName == "" ||
                x.CharacterClassName == null ||
                x.CharacterClassName == "" ||
                x.CharacterSpecName == null ||
                x.CharacterSpecName == "")
            .ToListAsync(cancellationToken);

        Guid[] characterIds =
        [
            .. scores.Select(x => x.CharacterId)
                .Concat(records.Select(x => x.CharacterId))
                .Distinct()
        ];

        if (characterIds.Length == 0)
        {
            return;
        }

        Dictionary<Guid, CharacterMetadata> characters = await LoadCharacterMetadataAsync(
            characterIds,
            cancellationToken);

        DateTime updatedAtUtc = DateTime.UtcNow;

        foreach (CharacterNominationScore score in scores)
        {
            if (!characters.TryGetValue(score.CharacterId, out CharacterMetadata? metadata))
            {
                continue;
            }

            score.UpdateParticipantMetadata(
                metadata.UserFirstName,
                metadata.UserLastName,
                metadata.CharacterClassName,
                metadata.CharacterSpecName,
                updatedAtUtc);
        }

        foreach (ChronicleRecord record in records)
        {
            if (!characters.TryGetValue(record.CharacterId, out CharacterMetadata? metadata))
            {
                continue;
            }

            record.UserFirstName = metadata.UserFirstName;
            record.UserLastName = metadata.UserLastName;
            record.CharacterClassName = metadata.CharacterClassName;
            record.CharacterSpecName = metadata.CharacterSpecName;
        }

        await chroniclesDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Dictionary<Guid, CharacterMetadata>> LoadCharacterMetadataAsync(
        Guid[] characterIds,
        CancellationToken cancellationToken)
    {
        var characters = await gameDbContext.Characters
            .AsNoTracking()
            .Where(x => characterIds.Contains(x.Id))
            .Select(x => new
            {
                x.Id,
                UserId = x.UserId.Value,
                CharacterClassName = x.Spec.Class.Name,
                CharacterSpecName = x.Spec.Name
            })
            .ToListAsync(cancellationToken);

        Guid[] userIds =
        [
            .. characters
                .Select(x => x.UserId)
                .Distinct()
        ];

        Dictionary<Guid, UserMetadata> users = await authDbContext.Users
            .AsNoTracking()
            .Where(x => userIds.Contains(x.Id))
            .Select(x => new
            {
                x.Id,
                x.FirstName,
                x.LastName
            })
            .ToDictionaryAsync(
                x => x.Id,
                x => new UserMetadata(x.FirstName, x.LastName),
                cancellationToken);

        return characters.ToDictionary(
            x => x.Id,
            x =>
            {
                users.TryGetValue(x.UserId, out UserMetadata? user);

                return new CharacterMetadata(
                    user?.FirstName,
                    user?.LastName,
                    x.CharacterClassName,
                    x.CharacterSpecName);
            });
    }

    private sealed record CharacterMetadata(
        string? UserFirstName,
        string? UserLastName,
        string? CharacterClassName,
        string? CharacterSpecName);

    private sealed record UserMetadata(string FirstName, string LastName);
}
