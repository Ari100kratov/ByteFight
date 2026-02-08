using Domain.GameRuntime.RuntimeLogEntries;
using GameRuntime.Core;
using GameRuntime.Core.Units;

namespace GameRuntime.Logic.Turns;

public interface IUnitTurnProcessor
{
    IEnumerable<RuntimeLogEntry> ProcessTurn(BaseUnit actor, ArenaWorld world);
}
