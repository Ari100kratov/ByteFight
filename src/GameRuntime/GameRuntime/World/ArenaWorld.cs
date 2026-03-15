using System.Reactive;
using Domain;
using Domain.GameRuntime.GameActionLogs;
using Domain.GameRuntime.GameResults;
using Domain.ValueObjects;
using GameRuntime.World.Units;
using SharedKernel;

namespace GameRuntime.World;

internal sealed class ArenaWorld
{
    public Guid GameSessionId { get; } = Guid.CreateVersion7();

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

    public IdleLogEntry CreateIdleLogEntry(BaseUnit actor, string? info)
        => new(GameSessionId, new UnitId(actor.Id), actor.Name, info, TurnIndex);

    public WalkLogEntry CreateWalkLogEntry(BaseUnit actor)
        => new(GameSessionId, new UnitId(actor.Id), actor.Name, null, actor.FacingDirection, actor.Position, TurnIndex);

    public AttackLogEntry CreateAttackLogEntry(BaseUnit actor, BaseUnit target, decimal damage, StatSnapshot targetHp)
        => new(GameSessionId, new UnitId(actor.Id), actor.Name, null, new UnitId(target.Id), target.Name, damage, actor.FacingDirection, targetHp, TurnIndex);

    public DeathLogEntry CreateDeathLogEntry(BaseUnit actor)
        => new(GameSessionId, new UnitId(actor.Id), actor.Name, null, TurnIndex);
}
