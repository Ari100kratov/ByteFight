using Chronicles.Application.Abstractions.Data;
using Chronicles.Domain;

namespace Chronicles.Application.Chronicles.ProcessCompletedGameSessions.Nominations;

/// <summary>
/// Рассчитывает номинацию самой быстрой победы.
/// </summary>
internal sealed class FastestVictoryProjector : IChronicleNominationProjector
{
    /// <inheritdoc />
    public IEnumerable<ChronicleNominationProjection> Project(
        CompletedGameSessionData session,
        IReadOnlyList<CompletedGameSessionParticipantData> playerParticipants)
    {
        CompletedGameSessionParticipantData? winner = playerParticipants
            .FirstOrDefault(x => x.UnitId == session.WinnerUnitId);

        if (winner is null)
        {
            yield break;
        }

        yield return ChronicleNominationProjection.ReplaceIfLower(
            ChronicleNominationType.FastestVictory,
            winner,
            session.TotalTurns,
            session.SessionId,
            session.EndedAtUtc);
    }
}
