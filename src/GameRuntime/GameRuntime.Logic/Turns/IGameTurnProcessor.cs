using Domain.GameRuntime.RuntimeLogEntries;
using GameRuntime.Core;

namespace GameRuntime.Logic.Turns;

public interface IGameTurnProcessor
{
    Task<TurnLog> ProcessTurn(ArenaWorld world);
}
