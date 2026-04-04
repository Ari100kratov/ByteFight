using Domain.GameRuntime.GameActionLogs;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Units;

namespace GameRuntime.Logic.Turns;

internal interface IUnitTurnProcessor
{
    IEnumerable<GameActionLogEntry> ProcessTurn(BaseUnit actor, ArenaWorld world);
}
