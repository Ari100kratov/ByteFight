using Domain.GameRuntime.RuntimeLogEntries;
using GameRuntime.Core;
using GameRuntime.Core.Units;

namespace GameRuntime.Logic.Turns;

public sealed class GameTurnProcessor : IGameTurnProcessor
{
    private readonly IUnitTurnProcessor _npcAi;
    private readonly IUnitTurnProcessor _playerAi;

    public GameTurnProcessor(IUnitTurnProcessor npcAi, IUnitTurnProcessor playerAi)
    {
        _npcAi = npcAi;
        _playerAi = playerAi;
    }

    public async Task<TurnLog> ProcessTurn(ArenaWorld world)
    {
        world.IncrementTurn();

        var logs = new List<RuntimeLogEntry>();

        if (!world.Player.IsDead)
        {
            logs.AddRange(_playerAi.ProcessTurn(world.Player, world));
        }

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

            logs.AddRange(_npcAi.ProcessTurn(enemy, world));
        }

        return new TurnLog
        {
            TurnIndex = world.TurnIndex,
            Logs = logs
        };
    }
}
