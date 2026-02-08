using Domain.GameRuntime.RuntimeLogEntries;
using GameRuntime.Core;
using GameRuntime.Core.Units;
using GameRuntime.Logic.Turns;
using GameRuntime.Logic.User.Api;

namespace GameRuntime.Logic.User.Execution;

public sealed class ScriptedUnitTurnProcessor : IUnitTurnProcessor
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
