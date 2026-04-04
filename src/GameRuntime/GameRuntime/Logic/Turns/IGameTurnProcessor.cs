using Domain.GameRuntime.GameActionLogs;
using GameRuntime.Common.World;

namespace GameRuntime.Logic.Turns;

internal interface IGameTurnProcessor
{
    Task<TurnLog> ProcessTurn(ArenaWorld world);
}
