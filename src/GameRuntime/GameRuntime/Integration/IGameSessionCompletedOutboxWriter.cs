using Domain.GameRuntime.GameSessions;

namespace GameRuntime.Integration;

internal interface IGameSessionCompletedOutboxWriter
{
    Task EnqueueAsync(GameSession session, CancellationToken cancellationToken);
}
