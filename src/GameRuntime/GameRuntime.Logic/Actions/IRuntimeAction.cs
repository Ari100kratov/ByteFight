using Domain.GameRuntime.RuntimeLogEntries;

namespace GameRuntime.Logic.Actions;

public interface IRuntimeAction
{
    IEnumerable<RuntimeLogEntry> Execute();
}
