using Chronicles.Application.Abstractions.Data;

namespace Chronicles.Application.Chronicles.ProcessCompletedGameSessions.Nominations;

/// <summary>
/// Стратегия расчёта одной или нескольких номинаций по завершённой игровой сессии.
/// </summary>
internal interface IChronicleNominationProjector
{
    /// <summary>
    /// Возвращает изменения номинаций, которые должна внести указанная игровая сессия.
    /// </summary>
    IEnumerable<ChronicleNominationProjection> Project(
        CompletedGameSessionData session,
        IReadOnlyList<CompletedGameSessionParticipantData> playerParticipants);
}
