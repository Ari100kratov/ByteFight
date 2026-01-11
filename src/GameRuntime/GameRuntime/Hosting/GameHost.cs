using System.Collections.Concurrent;
using Application.Abstractions.GameRuntime;
using GameRuntime.Builders;
using GameRuntime.World;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace GameRuntime.Hosting;

internal sealed class GameHost(
    ArenaWorldBuilder arenaWorldBuilder,
    GameInstanceFactory gameInstanceFactory,
    ILogger<GameHost> logger)
    : IGameHost
{
    private readonly ConcurrentDictionary<Guid, GameInstance> _running = new();

    public async Task<Result<Guid>> StartGame(GameInitModel initModel, CancellationToken ct)
    {
        try
        {
            Result<ArenaWorld> buildResult = await arenaWorldBuilder.Build(initModel.ArenaId, initModel.CharacterId, ct);
            if (buildResult.IsFailure)
            {
                return Result.Failure<Guid>(buildResult.Error);
            }

            GameInstance gameInstance = await gameInstanceFactory.Create(initModel, buildResult.Value, OnGameCompleted, ct);
            _running[gameInstance.SessionId] = gameInstance;

            // TODO: в будущем реализовать полноценные очереди
            _ = Task.Run(() => gameInstance.Run(), CancellationToken.None);

            return gameInstance.SessionId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to start game session. UserId {UserId}, ArenaId {ArenaId}, CharacterId {CharacterId}, Mode {Mode}",
                initModel.UserId, initModel.ArenaId, initModel.CharacterId, initModel.Mode);

            // TODO: Передавать причину ошибки?
            return Result.Failure<Guid>(GameHostErrors.StartFailure(initModel));
        }
    }

    private void OnGameCompleted(Guid sessionId)
    {
        _running.TryRemove(sessionId, out _);
    }

    public Result CancelGame(Guid sessionId)
    {
        if (_running.TryGetValue(sessionId, out GameInstance? instance))
        {
            instance.RequestCancel();
            return Result.Success();
        }

        return Result.Failure(GameHostErrors.NotFound(sessionId));
    }
}

internal static class GameHostErrors
{
    public static Error NotFound(Guid sessionId) =>
        Error.NotFound("GameHost.NotRunning", $"Игровая сессия с Id = {sessionId} не найдена.");

    public static Error StartFailure(GameInitModel initModel) => Error.Failure(
        "GameHost.StartFailed",
        $"Не удалось начать игровую сессию. UserId {initModel.UserId}, ArenaId {initModel.ArenaId}, CharacterId {initModel.CharacterId}, Mode {initModel.Mode}");
}
