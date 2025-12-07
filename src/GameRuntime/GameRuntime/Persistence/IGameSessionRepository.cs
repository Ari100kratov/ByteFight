using Application.Abstractions.GameRuntime;
using Domain.GameRuntime.GameResults;
using Domain.GameRuntime.GameSessions;

namespace GameRuntime.Persistence;

internal interface IGameSessionRepository
{
    Task<GameSession> Create(GameInitModel initModel, IEnumerable<Guid> arenaEnemyIds, CancellationToken ct);

    Task<GameSession> MarkStarted(Guid id);

    Task<GameSession> CompleteSuccess(Guid id, GameResult gameResult, int turns);

    Task<GameSession> CompleteWithError(Guid id, string? reason, int turns);

    Task<GameSession> Abort(Guid id, int turns);
}
