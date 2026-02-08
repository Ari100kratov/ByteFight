using Application.Abstractions.GameRuntime;
using Domain.GameRuntime.GameResults;
using Domain.GameRuntime.GameSessions;
using Domain.GameRuntime.RuntimeLogEntries;
using GameRuntime.Core;
using GameRuntime.Logic.Turns;
using GameRuntime.Persistence;
using Microsoft.Extensions.Logging;

namespace GameRuntime.Hosting;

internal sealed class GameInstance
{
    public Guid SessionId { get; }
    private readonly ArenaWorld _world;
    private readonly Action<Guid> _onCompleted;

    private readonly IGameSessionRepository _sessionRepository;
    private readonly IGameTurnProcessor _gameTurnProcessor;
    private readonly IGameRuntimeEventSender _eventSender;
    private readonly ILogger<GameInstance> _logger;

    public GameInstance(Guid sessionId,
        ArenaWorld world,
        Action<Guid> onCompleted,
        IGameSessionRepository sessionRepository,
        IGameTurnProcessor gameTurnProcessor,
        IGameRuntimeEventSender eventSender,
        ILogger<GameInstance> logger)
    {
        SessionId = sessionId;
        _world = world;
        _onCompleted = onCompleted;

        _sessionRepository = sessionRepository;
        _logger = logger;
        _gameTurnProcessor = gameTurnProcessor;
        _eventSender = eventSender;
    }

    private volatile bool _cancelRequested;

    public async Task Run()
    {
        try
        {
            await _sessionRepository.MarkStarted(SessionId);

            while (true)
            {
                await Task.Delay(1000);

                if (_cancelRequested)
                {
                    GameSession gameSession = await _sessionRepository.Abort(SessionId, _world.TurnIndex);
                    await _eventSender.SendFinished(gameSession, CancellationToken.None);
                    return;
                }

                TurnLog turnLog = await _gameTurnProcessor.ProcessTurn(_world);
                await _eventSender.SendTick(SessionId, turnLog, CancellationToken.None);

                GameResult? gameResult = _world.CheckGameOver();
                if (gameResult is not null)
                {
                    GameSession gameSession = await _sessionRepository.CompleteSuccess(SessionId, gameResult, _world.TurnIndex);
                    await _eventSender.SendFinished(gameSession, CancellationToken.None);

                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "GameInstance {SessionId} failed during execution. Turns={Turns}",
                SessionId, _world.TurnIndex);

            GameSession gameSession = await _sessionRepository.CompleteWithError(SessionId, ex.Message, _world.TurnIndex);
            await _eventSender.SendFinished(gameSession, CancellationToken.None);
        }
        finally
        {
            _onCompleted(SessionId);
        }
    }

    public void RequestCancel()
    {
        _cancelRequested = true;
    }
}
