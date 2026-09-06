using Application.Abstractions.GameRuntime;
using Domain.GameRuntime.GameActionLogs;
using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.GameRuntime.GameResults;
using Domain.GameRuntime.GameSessions;
using GameRuntime.Common.World;
using GameRuntime.Logic.Turns;
using GameRuntime.Persistence;
using GameRuntime.Realtime;
using Microsoft.Extensions.Logging;

namespace GameRuntime.Hosting;

/// <summary>
/// Запущенный бой: пошагово двигает <see cref="BattleEngine"/>,
/// сохраняет записи журнала, рассылает состояние и логи клиентам
/// и завершает сессию с результатом.
/// </summary>
internal sealed class GameInstance : IDisposable
{
    /// <summary>
    /// Пауза между шагами боя, чтобы клиент успевал проигрывать анимации.
    /// </summary>
    private static readonly TimeSpan StepDelay = TimeSpan.FromMilliseconds(450);

    public Guid SessionId { get; }

    private readonly ArenaWorld _world;
    private readonly BattleEngine _engine;
    private readonly Action<Guid> _onCompleted;
    private readonly BattleCommandRegistry _commandRegistry;
    private readonly IDisposable? _disposableResource;
    private bool _disposed;

    private readonly IGameSessionRepository _sessionRepository;
    private readonly IGameRuntimeEventSender _eventSender;
    private readonly ILogger<GameInstance> _logger;
    private readonly CancellationTokenSource _cancelSource = new();

    public GameInstance(Guid sessionId,
        ArenaWorld world,
        IBattleTurnRunner npcRunner,
        IBattleTurnRunner playerRunner,
        Action<Guid> onCompleted,
        BattleCommandRegistry commandRegistry,
        IDisposable? disposableResource,
        IGameSessionRepository sessionRepository,
        IGameRuntimeEventSender eventSender,
        ILogger<GameInstance> logger)
    {
        SessionId = sessionId;
        _world = world;
        _onCompleted = onCompleted;
        _commandRegistry = commandRegistry;
        _disposableResource = disposableResource;
        _sessionRepository = sessionRepository;
        _eventSender = eventSender;
        _logger = logger;

        // Движок создаётся здесь, чтобы получить колбэк немедленной рассылки.
        _engine = new BattleEngine(world, npcRunner, playerRunner, BroadcastAction);
    }

    public async Task Run()
    {
        try
        {
            await _sessionRepository.MarkStarted(SessionId);

            // Первое состояние — сразу после старта.
            await BroadcastState();

            while (!_cancelSource.IsCancellationRequested)
            {
                BattleStepResult step = await _engine.StepAsync(_cancelSource.Token);

                if (step.Logs.Count > 0)
                {
                    await PersistAndSend(step.Logs);
                }

                if (step.Finished)
                {
                    GameResult? result = _world.CheckGameOver();
                    GameSession gameSession = result is null
                        ? await _sessionRepository.Abort(SessionId, _world.TurnIndex)
                        : await _sessionRepository.CompleteSuccess(SessionId, result, _world.TurnIndex);

                    await _eventSender.SendFinished(gameSession, CancellationToken.None);
                    break;
                }

                await BroadcastState();
                await Task.Delay(StepDelay, CancellationToken.None);
            }

            if (_cancelSource.IsCancellationRequested)
            {
                GameSession gameSession =
                    await _sessionRepository.Abort(SessionId, _world.TurnIndex);
                await _eventSender.SendFinished(gameSession, CancellationToken.None);
            }
        }
        catch (OperationCanceledException)
        {
            GameSession gameSession =
                await _sessionRepository.Abort(SessionId, _world.TurnIndex);
            await _eventSender.SendFinished(gameSession, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "GameInstance {SessionId} failed during execution. Turns={Turns}",
                SessionId, _world.TurnIndex);

            GameSession gameSession =
                await _sessionRepository.CompleteWithError(SessionId, ex.Message, _world.TurnIndex);
            await _eventSender.SendFinished(gameSession, CancellationToken.None);
        }
        finally
        {
            try
            {
                Dispose();
            }
            finally
            {
                _onCompleted(SessionId);
            }
        }
    }

    /// <summary>
    /// Немедленно рассылает логи одного действия (для живого отклика
    /// в ручном режиме). Вызывается синхронно из движка боя.
    /// </summary>
    private void BroadcastAction(IReadOnlyList<GameActionLogEntry> logs) =>
        _ = PersistAndSend(logs);

    private async Task PersistAndSend(IReadOnlyList<GameActionLogEntry> logs)
    {
        await _sessionRepository.Save(logs);

        await _eventSender.SendTick(
            SessionId,
            new TurnLog { TurnIndex = _world.TurnIndex, Logs = logs },
            CancellationToken.None);
    }

    private Task BroadcastState() =>
        _eventSender.SendState(SessionId, BattleStateBuilder.Build(_world), CancellationToken.None);

    public void RequestCancel() => _cancelSource.Cancel();

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _cancelSource.Cancel();
        _cancelSource.Dispose();
        _commandRegistry.Unregister(SessionId);
        _disposableResource?.Dispose();
    }
}
