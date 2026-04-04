using System.Collections.Concurrent;
using Application.Abstractions.GameRuntime;
using GameRuntime.Builders;
using GameRuntime.Common.World;
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

    /// <summary>
    /// Множество пользователей, у которых прямо сейчас есть активная
    /// или запускающаяся игровая сессия.
    /// </summary>
    private readonly ConcurrentDictionary<Guid, byte> _usersInGame = new();

    public async Task<Result<Guid>> StartGame(GameInitModel initModel, CancellationToken ct)
    {
        // Атомарно резервируем слот пользователя.
        if (!_usersInGame.TryAdd(initModel.UserId, 0))
        {
            return Result.Failure<Guid>(GameHostErrors.AlreadyRunningForUser(initModel.UserId));
        }

        try
        {
            Result<ArenaWorld> buildResult =
                await arenaWorldBuilder.Build(initModel.ArenaId, initModel.CharacterId, ct);

            if (buildResult.IsFailure)
            {
                _usersInGame.TryRemove(initModel.UserId, out _);
                return Result.Failure<Guid>(buildResult.Error);
            }

            Result<GameInstance> createResult =
                await gameInstanceFactory.Create(
                    initModel,
                    buildResult.Value,
                    sessionId => OnGameCompleted(initModel.UserId, sessionId),
                    ct);

            if (createResult.IsFailure)
            {
                _usersInGame.TryRemove(initModel.UserId, out _);
                return Result.Failure<Guid>(createResult.Error);
            }

            GameInstance gameInstance = createResult.Value;

            if (!_running.TryAdd(gameInstance.SessionId, gameInstance))
            {
                _usersInGame.TryRemove(initModel.UserId, out _);
                return Result.Failure<Guid>(GameHostErrors.SessionAlreadyRegistered(gameInstance.SessionId));
            }

            // TODO: в будущем реализовать полноценные очереди
            _ = Task.Run(async () =>
            {
                try
                {
                    await gameInstance.Run();
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Unhandled error while running game session {SessionId} for user {UserId}",
                        gameInstance.SessionId,
                        initModel.UserId);

                    OnGameCompleted(initModel.UserId, gameInstance.SessionId);
                }
            }, CancellationToken.None);

            return gameInstance.SessionId;
        }
        catch (OperationCanceledException)
        {
            _usersInGame.TryRemove(initModel.UserId, out _);
            throw;
        }
        catch (Exception ex)
        {
            _usersInGame.TryRemove(initModel.UserId, out _);

            logger.LogError(ex,
                "Failed to start game session. UserId {UserId}, ArenaId {ArenaId}, CharacterId {CharacterId}, Mode {Mode}",
                initModel.UserId, initModel.ArenaId, initModel.CharacterId, initModel.Mode);

            return Result.Failure<Guid>(GameHostErrors.StartFailure(initModel));
        }
    }

    private void OnGameCompleted(Guid userId, Guid sessionId)
    {
        _running.TryRemove(sessionId, out _);
        _usersInGame.TryRemove(userId, out _);
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
