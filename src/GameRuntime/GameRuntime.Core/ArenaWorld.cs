using Domain.GameRuntime.GameResults;
using GameRuntime.Core.Units;
using SharedKernel;

namespace GameRuntime.Core;

public sealed class ArenaWorld
{
    public required ArenaDefinition Arena { get; init; }

    public required PlayerUnit Player { get; init; }

    public required IReadOnlyList<EnemyUnit> Enemies { get; init; }

    public int TurnIndex { get; private set; }

    public void IncrementTurn() => TurnIndex++;

    public BaseUnit GetUnit(Guid unitId)
    {
        if (Player.CharacterId == unitId)
        {
            return Player;
        }

        EnemyUnit? enemy = Enemies.FirstOrDefault(x => x.ArenaEnemyId == unitId);
        if (enemy is not null)
        {
            return enemy;
        }

        throw new DomainException(
            "UNIT_NOT_FOUND",
            $"Unit with id {unitId} was not found in this arena."
        );
    }

    public GameResult? CheckGameOver()
    {
        bool allEnemiesDead = Enemies.All(e => e.IsDead);
        bool playerDead = Player.IsDead;

        if (allEnemiesDead && playerDead)
        {
            return GameResult.Draw();
        }

        if (playerDead)
        {
            if (Player.KilledByUnitId is null)
            {
                throw new DomainException(
                    "GAME_RESULT_INVALID_KILLER",
                    "Player is dead, but no killer unit was specified. Game result cannot be constructed."
                );
            }

            return GameResult.EnemyVictory(Player.KilledByUnitId.Value);
        }

        if (allEnemiesDead)
        {
            return GameResult.PlayerVictory(Player.Id);
        }

        if (TurnIndex >= Arena.MaxTurnsCount)
        {
            return GameResult.TurnLimitLoss();
        }

        return null;
    }
}
