using Chronicles.Application.Abstractions.Data;
using Chronicles.Domain;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Chronicles.Application.Chronicles.ProcessCompletedGameSessions;

internal sealed class ChroniclesProjectionSessionProcessor(
    IChroniclesDbContext chroniclesDbContext,
    IEnumerable<IChronicleNominationProjector> nominationProjectors,
    IDateTimeProvider dateTimeProvider)
{
    public async Task<ChroniclesProjectionSessionResult> ApplyAsync(
        CompletedGameSessionData session,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<CompletedGameSessionParticipantData> playerParticipants =
            [.. session.Participants.Where(x => x.UnitType == CompletedParticipantUnitType.Player)];

        int updatedCharacters = await ApplyCharacterStatsAsync(session, playerParticipants, cancellationToken);
        int createdRecords = 0;
        int updatedScores = 0;

        var projections = nominationProjectors
            .SelectMany(projector => projector.Project(session, playerParticipants))
            .Where(x => x.Value > 0)
            .ToList();

        if (projections.Count == 0)
        {
            return new ChroniclesProjectionSessionResult(updatedCharacters, createdRecords, updatedScores);
        }

        Guid[] characterIds = [.. projections
            .Select(x => x.CharacterId)
            .Distinct()];

        ChronicleNominationType[] nominationTypes = [.. projections
            .Select(x => x.NominationType)
            .Distinct()];

        Dictionary<(ChronicleNominationType NominationType, Guid CharacterId), CharacterNominationScore> scores =
            await chroniclesDbContext.CharacterNominationScores
                .Where(x => characterIds.Contains(x.CharacterId) && nominationTypes.Contains(x.NominationType))
                .ToDictionaryAsync(x => (x.NominationType, x.CharacterId), cancellationToken);

        DateTime now = dateTimeProvider.UtcNow;

        foreach (ChronicleNominationProjection projection in projections)
        {
            if (projection.CreateRecord)
            {
                chroniclesDbContext.ChronicleRecords.Add(ChronicleRecord.Create(
                    projection.CharacterId,
                    projection.CharacterName,
                    projection.UserFirstName,
                    projection.UserLastName,
                    projection.CharacterClassName,
                    projection.CharacterSpecName,
                    projection.SessionId,
                    projection.NominationType,
                    projection.Value,
                    projection.OccurredAtUtc));

                createdRecords++;
            }

            (ChronicleNominationType NominationType, Guid CharacterId) key = (projection.NominationType, projection.CharacterId);

            if (!scores.TryGetValue(key, out CharacterNominationScore? score))
            {
                score = CharacterNominationScore.Create(
                    projection.CharacterId,
                    projection.CharacterName,
                    projection.NominationType,
                    now);

                chroniclesDbContext.CharacterNominationScores.Add(score);
                scores.Add(key, score);
            }

            ApplyScoreUpdate(score, projection, now);
            updatedScores++;
        }

        return new ChroniclesProjectionSessionResult(updatedCharacters, createdRecords, updatedScores);
    }

    private static void ApplyScoreUpdate(
        CharacterNominationScore score,
        ChronicleNominationProjection projection,
        DateTime updatedAtUtc)
    {
        switch (projection.ScoreUpdateMode)
        {
            case ChronicleNominationScoreUpdateMode.Increment:
                score.Add(
                    projection.Value,
                    projection.CharacterName,
                    projection.UserFirstName,
                    projection.UserLastName,
                    projection.CharacterClassName,
                    projection.CharacterSpecName,
                    projection.SessionId,
                    projection.OccurredAtUtc,
                    updatedAtUtc);
                break;
            case ChronicleNominationScoreUpdateMode.ReplaceIfGreater:
                score.ReplaceIfGreater(
                    projection.Value,
                    projection.CharacterName,
                    projection.UserFirstName,
                    projection.UserLastName,
                    projection.CharacterClassName,
                    projection.CharacterSpecName,
                    projection.SessionId,
                    projection.OccurredAtUtc,
                    updatedAtUtc);
                break;
            case ChronicleNominationScoreUpdateMode.ReplaceIfLower:
                score.ReplaceIfLower(
                    projection.Value,
                    projection.CharacterName,
                    projection.UserFirstName,
                    projection.UserLastName,
                    projection.CharacterClassName,
                    projection.CharacterSpecName,
                    projection.SessionId,
                    projection.OccurredAtUtc,
                    updatedAtUtc);
                break;
            default:
                throw new InvalidOperationException($"Unsupported score update mode '{projection.ScoreUpdateMode}'.");
        }
    }

    private async Task<int> ApplyCharacterStatsAsync(
        CompletedGameSessionData session,
        IReadOnlyList<CompletedGameSessionParticipantData> playerParticipants,
        CancellationToken cancellationToken)
    {
        if (playerParticipants.Count == 0)
        {
            return 0;
        }

        Guid? winnerCharacterId = playerParticipants
            .Where(x => x.UnitId == session.WinnerUnitId)
            .Select(x => (Guid?)x.UnitId)
            .FirstOrDefault();

        Guid[] characterIds = [.. playerParticipants
            .Select(x => x.UnitId)
            .Distinct()];

        Dictionary<Guid, CharacterChronicleStats> statsByCharacterId = await chroniclesDbContext.CharacterChronicleStats
            .Where(x => characterIds.Contains(x.CharacterId))
            .ToDictionaryAsync(x => x.CharacterId, cancellationToken);

        foreach (Guid characterId in characterIds)
        {
            if (statsByCharacterId.ContainsKey(characterId))
            {
                continue;
            }

            string characterName = playerParticipants.Single(x => x.UnitId == characterId).DisplayName;
            var stats = CharacterChronicleStats.Create(characterId, characterName);
            chroniclesDbContext.CharacterChronicleStats.Add(stats);
            statsByCharacterId.Add(characterId, stats);
        }

        foreach (CompletedGameSessionParticipantData participant in playerParticipants)
        {
            statsByCharacterId[participant.UnitId].ApplySession(
                participant.DisplayName,
                session.EndedAtUtc,
                session.TotalTurns,
                winnerCharacterId == participant.UnitId,
                session.Outcome == CompletedGameSessionOutcome.Draw);
        }

        return playerParticipants.Count;
    }
}
