using Domain.GameRuntime.GameActionLogs;
using GameRuntime.World;
using GameRuntime.World.Units;

namespace GameRuntime.Logic.Turns;

internal interface IUnitTurnProcessor
{
    IEnumerable<GameActionLogEntry> ProcessTurn(BaseUnit actor, ArenaWorld world);
}
