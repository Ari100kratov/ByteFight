using Application.Abstractions.GameRuntime;
using Domain.GameRuntime.GameSessions;
using GameRuntime.Common.World;
using GameRuntime.Hosting;
using GameRuntime.Logic.NPC;
using GameRuntime.Logic.Turns;
using GameRuntime.Logic.User.Compilation;
using GameRuntime.Logic.User.Execution;
using GameRuntime.Persistence;
using GameRuntime.Realtime;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace GameRuntime.Builders;

/// <summary>
/// Создаёт боевую сессию. Ручной режим — основной: игрок управляет
/// персонажем сам. Скриптовый режим включается, когда передан код
/// (функция скрыта из интерфейса, но сохранена для совместимости).
/// </summary>
internal sealed class GameInstanceFactory(
    TacticalEnemyAiRunner npcRunner,
    UserActionExecutor executor,
    IUserCodeRunner userCodeRunner,
    UserScriptCompiler compiler,
    UserCodeExecutionOptions userCodeExecutionOptions,
    BattleCommandRegistry commandRegistry,
    IGameSessionRepository sessionRepository,
    IGameRuntimeEventSender events,
    ILoggerFactory loggerFactory)
{
    public async Task<Result<GameInstance>> Create(
        GameInitModel initModel,
        ArenaWorld world,
        Action<Guid> onCompleted,
        CancellationToken ct)
    {
        CompiledUserScript? compiledScript = null;
        ScriptedPlayerTurnRunner? scriptedRunner = null;

        try
        {
            IBattleTurnRunner playerRunner;
            IDisposable? ownedResource;

            if (string.IsNullOrWhiteSpace(initModel.Code))
            {
                // Ручной режим: ждём команды игрока через хаб.
                BattleCommandQueue queue = commandRegistry.Register(world.GameSessionId);

                playerRunner = new ManualPlayerTurnRunner(queue);
                ownedResource = null;
            }
            else
            {
                try
                {
                    compiledScript = compiler.Compile(initModel.Code);
                }
                catch (Exception ex)
                {
                    return Result.Failure<GameInstance>(
                        GameHostErrors.UserCodeCompilationFailed(
                            $"Не удалось скомпилировать пользовательский код: {ex.Message}"));
                }

                scriptedRunner = new ScriptedPlayerTurnRunner(
                    compiledScript,
                    userCodeRunner,
                    executor,
                    userCodeExecutionOptions);

                // Владение compiledScript перешло в scriptedRunner.
                compiledScript = null;

                playerRunner = scriptedRunner;
                ownedResource = scriptedRunner;
            }

            IEnumerable<GameSessionParticipantInitModel> arenaEnemies = world.Enemies
                .Select(x => new GameSessionParticipantInitModel(x.ArenaEnemyId, x.Name));

            GameSession gameSession = await sessionRepository.Create(
                world.GameSessionId,
                initModel,
                world.Player.Name,
                arenaEnemies,
                ct);

            var gameInstance = new GameInstance(
                sessionId: gameSession.Id,
                world: world,
                npcRunner: npcRunner,
                playerRunner: playerRunner,
                onCompleted: onCompleted,
                commandRegistry: commandRegistry,
                disposableResource: ownedResource,
                sessionRepository: sessionRepository,
                eventSender: events,
                logger: loggerFactory.CreateLogger<GameInstance>());

            // Владение scriptedRunner перешло в gameInstance.
            scriptedRunner = null;

            return gameInstance;
        }
        finally
        {
            scriptedRunner?.Dispose();
            compiledScript?.Dispose();
        }
    }
}
