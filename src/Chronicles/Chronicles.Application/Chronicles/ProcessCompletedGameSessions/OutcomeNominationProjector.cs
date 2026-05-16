using Chronicles.Application.Abstractions.Data;
using Chronicles.Domain;

namespace Chronicles.Application.Chronicles.ProcessCompletedGameSessions;

internal sealed class OutcomeNominationProjector : IChronicleNominationProjector
{
    public IEnumerable<ChronicleNominationProjection> Project(
        CompletedGameSessionData session,
        IReadOnlyList<CompletedGameSessionParticipantData> playerParticipants)
    {
        if (playerParticipants.Count == 0)
        {
            yield break;
        }

        Guid? winnerCharacterId = playerParticipants
            .Where(x => x.UnitId == session.WinnerUnitId)
            .Select(x => (Guid?)x.UnitId)
            .FirstOrDefault();

        foreach (CompletedGameSessionParticipantData participant in playerParticipants)
        {
            yield return CreateIncrement(
                ChronicleNominationType.MostBattlesPlayed,
                participant,
                session);

            if (winnerCharacterId == participant.UnitId)
            {
                yield return CreateIncrement(
                    ChronicleNominationType.MostVictories,
                    participant,
                    session);

                continue;
            }

            if (session.Outcome == CompletedGameSessionOutcome.Draw)
            {
                yield return CreateIncrement(
                    ChronicleNominationType.MostDraws,
                    participant,
                    session);

                continue;
            }

            yield return CreateIncrement(
                ChronicleNominationType.MostDefeats,
                participant,
                session);
        }
    }

    private static ChronicleNominationProjection CreateIncrement(
        ChronicleNominationType nominationType,
        CompletedGameSessionParticipantData participant,
        CompletedGameSessionData session) =>
        ChronicleNominationProjection.Increment(
            nominationType,
            participant,
            1,
            session.SessionId,
            session.EndedAtUtc);
}
