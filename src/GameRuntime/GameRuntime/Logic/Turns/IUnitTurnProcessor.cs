using Domain.GameRuntime.RuntimeLogEntries;
using GameRuntime.World;
using GameRuntime.World.Units;

namespace GameRuntime.Logic.Turns;

internal interface IUnitTurnProcessor
{
    IEnumerable<RuntimeLogEntry> ProcessTurn(BaseUnit actor, ArenaWorld world);
}
