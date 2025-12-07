using Domain.GameRuntime.RuntimeLogEntries;
using GameRuntime.Logic.NPC;
using GameRuntime.World;
using GameRuntime.World.Units;

namespace GameRuntime.Logic.Turns;

internal sealed class GameTurnProcessor : IGameTurnProcessor
{
    private readonly IUnitTurnProcessor _unitAi;

    public GameTurnProcessor(IUnitTurnProcessor unitAi)
    {
        _unitAi = unitAi;
    }

    public async Task<TurnLog> ProcessTurn(ArenaWorld world)
    {
        world.IncrementTurn();

        IEnumerable<RuntimeLogEntry> logs = EnemiesTurn(world);

        return new TurnLog
        {
            TurnIndex = world.TurnIndex,
            Logs = [.. logs]
        };
    }

    private IEnumerable<RuntimeLogEntry> EnemiesTurn(ArenaWorld world)
    {
        foreach (EnemyUnit enemy in world.Enemies)
        {
            if (world.Player.IsDead)
            {
                break;
            }

            if (enemy.IsDead)
            {
                continue;
            }

            foreach (RuntimeLogEntry logEntry in _unitAi.ProcessTurn(enemy, world))
            {
                yield return logEntry;
            }
        }
    }
}
