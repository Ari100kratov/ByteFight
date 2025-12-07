using Domain.GameRuntime.RuntimeLogEntries;

namespace GameRuntime.Logic.Actions;

internal interface IRuntimeAction
{
    IEnumerable<RuntimeLogEntry> Execute();
}
