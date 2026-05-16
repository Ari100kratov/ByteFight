using Chronicles.Application.Abstractions.Data;

namespace Chronicles.Application.Chronicles.ProcessCompletedGameSessions;

internal interface IChronicleNominationProjector
{
    IEnumerable<ChronicleNominationProjection> Project(
        CompletedGameSessionData session,
        IReadOnlyList<CompletedGameSessionParticipantData> playerParticipants);
}
