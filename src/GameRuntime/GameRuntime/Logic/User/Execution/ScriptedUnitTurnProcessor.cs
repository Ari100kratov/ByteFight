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
        UserAction action;

        try
        {
            action = _decide(view);
        }
        catch (Exception ex)
        {
            return [world.CreateIdleLogEntry(actor.Id, IdleReasons.UserError(ex.Message))];
        }

        return _executor.Execute(action, actor, world);
    }
}
