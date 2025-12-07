using Domain.GameRuntime.RuntimeLogEntries;
using GameRuntime.World;

namespace GameRuntime.Logic.Turns;

internal interface IGameTurnProcessor
{
    Task<TurnLog> ProcessTurn(ArenaWorld world);
}
