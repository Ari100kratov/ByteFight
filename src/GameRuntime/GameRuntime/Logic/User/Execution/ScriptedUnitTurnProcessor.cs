using Domain.GameRuntime.GameActionLogs;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Units;
using GameRuntime.Logic.Turns;
using GameRuntime.Logic.User.Api;
using GameRuntime.Logic.User.Compilation;

namespace GameRuntime.Logic.User.Execution;

internal sealed class ScriptedUnitTurnProcessor : IUnitTurnProcessor, IDisposable
{
    private readonly CompiledUserScript _script;
    private readonly IUserCodeRunner _runner;
    private readonly UserActionExecutor _executor;
    private readonly UserCodeExecutionOptions _options;
    private bool _disposed;

    public ScriptedUnitTurnProcessor(
        CompiledUserScript script,
        IUserCodeRunner runner,
        UserActionExecutor executor,
        UserCodeExecutionOptions options)
    {
        _script = script;
        _runner = runner;
        _executor = executor;
        _options = options;
    }

    public IEnumerable<GameActionLogEntry> ProcessTurn(BaseUnit actor, ArenaWorld world)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        UserWorldView view = world.ToView(actor);

        try
        {
            UserAction action = _runner.Run(
                _script,
                view,
                _options.Timeout);

            return _executor.Execute(action, actor, world);
        }
        catch (TimeoutException)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.Timeout(_options.Timeout))];
        }
        catch (InvalidOperationException ex)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.UserError(ex.Message))];
        }
        catch (Exception ex)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.UserError(
                $"Внутренняя ошибка выполнения пользовательского кода: {ex.Message}"))];
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _script.Dispose();
    }
}
