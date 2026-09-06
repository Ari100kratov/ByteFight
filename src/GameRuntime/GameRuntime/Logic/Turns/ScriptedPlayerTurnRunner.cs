using Domain.GameRuntime.GameActionLogs.Entries;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Units;
using GameRuntime.Logic.User.Compilation;
using GameRuntime.Logic.User.Execution;

namespace GameRuntime.Logic.Turns;

/// <summary>
/// Адаптер исполнения пользовательского кода под интерфейс раннера хода.
/// Скриптовый режим оставлен для совместимости и скрыт из интерфейса игры.
/// </summary>
internal sealed class ScriptedPlayerTurnRunner : IBattleTurnRunner, IDisposable
{
    private readonly ScriptedUnitTurnProcessor processor;

    public ScriptedPlayerTurnRunner(
        CompiledUserScript script,
        IUserCodeRunner runner,
        UserActionExecutor executor,
        UserCodeExecutionOptions options)
    {
        processor = new ScriptedUnitTurnProcessor(script, runner, executor, options);
    }

    public Task<IReadOnlyList<GameActionLogEntry>> PlayTurn(
        BaseUnit unit,
        ArenaWorld world,
        Action<IReadOnlyList<GameActionLogEntry>> onActionPerformed,
        CancellationToken ct)
    {
        IReadOnlyList<GameActionLogEntry> logs = [.. processor.ProcessTurn(unit, world)];

        if (logs.Count > 0)
        {
            onActionPerformed(logs);
        }

        return Task.FromResult(logs);
    }

    public void Dispose() => processor.Dispose();
}
