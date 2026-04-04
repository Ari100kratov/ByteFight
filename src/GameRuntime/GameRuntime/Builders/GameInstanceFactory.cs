using Application.Abstractions.GameRuntime;
using Domain.GameRuntime.GameSessions;
using GameRuntime.Common.World;
using GameRuntime.Hosting;
using GameRuntime.Logic.NPC;
using GameRuntime.Logic.Turns;
using GameRuntime.Logic.User.Compilation;
using GameRuntime.Logic.User.Execution;
using GameRuntime.Persistence;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace GameRuntime.Builders;

internal sealed class GameInstanceFactory
{
    private readonly BasicEnemyAiProcessor _npcAi;
    private readonly UserActionExecutor _executor;
    private readonly IUserCodeRunner _userCodeRunner;
    private readonly UserScriptCompiler _compiler;
    private readonly UserCodeExecutionOptions _userCodeExecutionOptions;
    private readonly IGameSessionRepository _sessionRepository;
    private readonly IGameRuntimeEventSender _events;
    private readonly ILoggerFactory _loggerFactory;

    public GameInstanceFactory(
        BasicEnemyAiProcessor npcAi,
        UserActionExecutor executor,
        IUserCodeRunner userCodeRunner,
        UserScriptCompiler compiler,
        UserCodeExecutionOptions userCodeExecutionOptions,
        IGameSessionRepository sessionRepository,
        IGameRuntimeEventSender events,
        ILoggerFactory loggerFactory)
    {
        _npcAi = npcAi;
        _executor = executor;
        _userCodeRunner = userCodeRunner;
        _compiler = compiler;
        _userCodeExecutionOptions = userCodeExecutionOptions;
        _sessionRepository = sessionRepository;
        _events = events;
        _loggerFactory = loggerFactory;
    }

    public async Task<Result<GameInstance>> Create(
        GameInitModel initModel,
        ArenaWorld world,
        Action<Guid> onCompleted,
        CancellationToken ct)
    {
        CompiledUserScript? compiledScript = null;
        ScriptedUnitTurnProcessor? playerAi = null;

        try
        {
            try
            {
                compiledScript = _compiler.Compile(initModel.Code);
            }
            catch (Exception ex)
            {
                return Result.Failure<GameInstance>(
                    GameHostErrors.UserCodeCompilationFailed(
                        $"Не удалось скомпилировать пользовательский код: {ex.Message}"));
            }

            playerAi = new ScriptedUnitTurnProcessor(
                compiledScript,
                _userCodeRunner,
                _executor,
                _userCodeExecutionOptions);

            // Владение compiledScript перешло в playerAi.
            compiledScript = null;

            var turnProcessor = new GameTurnProcessor(_npcAi, playerAi);

            IEnumerable<Guid> arenaEnemyIds = world.Enemies.Select(x => x.ArenaEnemyId);
            GameSession gameSession = await _sessionRepository.Create(
                world.GameSessionId,
                initModel,
                arenaEnemyIds,
                ct);

            var gameInstance = new GameInstance(
                sessionId: gameSession.Id,
                world: world,
                onCompleted: onCompleted,
                disposableResource: playerAi,
                sessionRepository: _sessionRepository,
                gameTurnProcessor: turnProcessor,
                eventSender: _events,
                logger: _loggerFactory.CreateLogger<GameInstance>());

            // Владение playerAi перешло в gameInstance.
            playerAi = null;

            return gameInstance;
        }
        finally
        {
            playerAi?.Dispose();
            compiledScript?.Dispose();
        }
    }
}
