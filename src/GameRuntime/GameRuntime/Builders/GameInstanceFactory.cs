using Application.Abstractions.GameRuntime;
using Domain.GameRuntime.GameSessions;
using GameRuntime.Hosting;
using GameRuntime.Logic.NPC;
using GameRuntime.Logic.Turns;
using GameRuntime.Logic.User.Api;
using GameRuntime.Logic.User.Compilation;
using GameRuntime.Logic.User.Execution;
using GameRuntime.Persistence;
using GameRuntime.World;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace GameRuntime.Builders;

internal sealed class GameInstanceFactory
{
    private readonly BasicEnemyAiProcessor _npcAi;
    private readonly UserActionExecutor _executor;
    private readonly UserScriptCompiler _compiler;
    private readonly IGameSessionRepository _sessionRepository;
    private readonly IGameRuntimeEventSender _events;
    private readonly ILoggerFactory _loggerFactory;

    public GameInstanceFactory(
        BasicEnemyAiProcessor npcAi,
        UserActionExecutor executor,
        UserScriptCompiler compiler,
        IGameSessionRepository sessionRepository,
        IGameRuntimeEventSender events,
        ILoggerFactory loggerFactory)
    {
        _npcAi = npcAi;
        _executor = executor;
        _compiler = compiler;
        _sessionRepository = sessionRepository;
        _events = events;
        _loggerFactory = loggerFactory;
    }

    public async Task<GameInstance> Create(
        GameInitModel initModel,
        ArenaWorld world,
        Action<Guid> onCompleted,
        CancellationToken ct)
    {
        // Компилируем код пользователя
        Func<UserWorldView, UserAction> decide;

        try
        {
            decide = _compiler.Compile(initModel.Code);
        }
        catch (Exception ex)
        {
            throw new DomainException("USER_CODE_COMPILATION_FAILED", ex.Message);
        }

        // Создаём player AI
        var playerAi = new ScriptedUnitTurnProcessor(decide, _executor);
        var turnProcessor = new GameTurnProcessor(_npcAi, playerAi);

        // Создаем игровую сессию
        IEnumerable<Guid> arenaEnemyIds = world.Enemies.Select(x => x.ArenaEnemyId);
        GameSession gameSession = await _sessionRepository.Create(initModel, arenaEnemyIds, ct);

        // Создаём GameInstance
        return new GameInstance(
            sessionId: gameSession.Id,
            world: world,
            onCompleted: onCompleted,
            sessionRepository: _sessionRepository,
            gameTurnProcessor: turnProcessor,
            eventSender: _events,
            logger: _loggerFactory.CreateLogger<GameInstance>()
        );
    }
}
