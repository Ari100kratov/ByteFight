using Chronicles.Application.Abstractions.Data;
using Chronicles.Domain;

namespace Chronicles.Application.Chronicles.ProcessCompletedGameSessions;

internal sealed class LongestBattleProjector : IChronicleNominationProjector
{
    public IEnumerable<ChronicleNominationProjection> Project(
        CompletedGameSessionData session,
        IReadOnlyList<CompletedGameSessionParticipantData> playerParticipants)
    {
        if (playerParticipants.Count == 0)
        {
            yield break;
        }

        foreach (CompletedGameSessionParticipantData participant in playerParticipants)
        {
            yield return ChronicleNominationProjection.ReplaceIfGreater(
                ChronicleNominationType.LongestBattle,
                participant,
                session.TotalTurns,
                session.SessionId,
                session.EndedAtUtc);
        }
    }
}
