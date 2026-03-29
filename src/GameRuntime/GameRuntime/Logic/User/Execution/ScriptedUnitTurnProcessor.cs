using Domain.GameRuntime.GameActionLogs;
using GameRuntime.Logic.Turns;
using GameRuntime.Logic.User.Api;
using GameRuntime.World;
using GameRuntime.World.Units;

namespace GameRuntime.Logic.User.Execution;

internal sealed class ScriptedUnitTurnProcessor : IUnitTurnProcessor
{
    private readonly Func<UserWorldView, UserAction> _decide;
    private readonly UserActionExecutor _executor;

    public ScriptedUnitTurnProcessor(
        Func<UserWorldView, UserAction> decide,
        UserActionExecutor executor)
    {
        _decide = decide;
        _executor = executor;
    }

    public IEnumerable<GameActionLogEntry> ProcessTurn(BaseUnit actor, ArenaWorld world)
    {
        UserWorldView view = world.ToView(actor);

        try
        {
            UserAction action = ExecuteWithTimeout(() => _decide(view), TimeSpan.FromSeconds(3));

            return _executor.Execute(action, actor, world);
        }
        catch (TimeoutException)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.Timeout)];
        }
        catch (Exception ex)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.UserError(ex.Message))];
        }
    }

    private static T ExecuteWithTimeout<T>(Func<T> func, TimeSpan timeout)
    {
        Task<T> task = Task.Run(func);

        if (task.Wait(timeout))
        {
            // пробрасываем исключение если было
            return task.GetAwaiter().GetResult();
        }

        throw new TimeoutException();
    }
}
