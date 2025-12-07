using Application.Abstractions.GameRuntime;
using Domain.GameRuntime.GameSessions;
using GameRuntime.Hosting;
using GameRuntime.Logic.Turns;
using GameRuntime.Persistence;
using GameRuntime.World;
using Microsoft.Extensions.Logging;

namespace GameRuntime.Builders;

internal sealed class GameInstanceFactory(
    IGameSessionRepository sessionRepository,
    IGameTurnProcessor gameTurnProcessor,
    IGameRuntimeEventSender eventSender,
    ILogger<GameInstance> logger)
{
    public async Task<GameInstance> Create(
        GameInitModel initModel,
        ArenaWorld arenaWorld,
        Action<Guid> onCompleted,
        CancellationToken ct)
    {
        IEnumerable<Guid> arenaEnemyIds = arenaWorld.Enemies.Select(x => x.ArenaEnemyId);
        GameSession gameSession = await sessionRepository.Create(initModel, arenaEnemyIds, ct);

        var gameInstance = new GameInstance(
            gameSession.Id,
            arenaWorld,
            onCompleted,
            sessionRepository,
            gameTurnProcessor,
            eventSender,
            logger);

        return gameInstance;
    }
}
