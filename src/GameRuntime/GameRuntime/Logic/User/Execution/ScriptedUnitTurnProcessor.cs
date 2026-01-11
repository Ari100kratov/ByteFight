using Domain.GameRuntime.RuntimeLogEntries;
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

    public IEnumerable<RuntimeLogEntry> ProcessTurn(BaseUnit actor, ArenaWorld world)
    {
        UserWorldView view = world.ToView(actor);
        UserAction action;

        try
        {
            action = _decide(view);
        }
        catch
        {
            // need log
            return [new IdleLogEntry(actor.Id)];
        }

        return _executor.Execute(action, actor, world);
    }
}
