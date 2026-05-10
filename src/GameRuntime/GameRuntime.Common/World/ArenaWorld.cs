using System.Collections.Immutable;
using Domain;
using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.GameRuntime.GameResults;
using Domain.ValueObjects;
using GameRuntime.Common.World.Abilities;
using GameRuntime.Common.World.ArenaItems;
using GameRuntime.Common.World.Units;
using SharedKernel;

namespace GameRuntime.Common.World;

public sealed class ArenaWorld
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

    public AbilityUsedLogEntry CreateAbilityUsedLogEntry(
        BaseUnit actor,
        BaseUnit target,
        RuntimeAbility ability,
        decimal value,
        StatSnapshot targetHp)
        => new(
            GameSessionId,
            new UnitId(actor.Id),
            actor.Name,
            null,
            ability.Type,
            ability.EffectType,
            ability.Name,
            new UnitId(target.Id),
            target.Name,
            value,
            actor.FacingDirection,
            targetHp,
            TurnIndex);

    public DeathLogEntry CreateDeathLogEntry(BaseUnit actor)
        => new(GameSessionId, new UnitId(actor.Id), actor.Name, null, TurnIndex);

    public ItemPickedUpLogEntry CreateItemPickedUpLogEntry(
        BaseUnit actor,
        ArenaItemDefinition item,
        decimal value,
        StatSnapshot actorHp)
        => new(
            GameSessionId,
            new UnitId(actor.Id),
            actor.Name,
            null,
            item.PlacedItemId,
            item.ItemId,
            item.Name,
            item.Type,
            item.Position,
            value,
            actorHp,
            TurnIndex);

    /// <summary>
    /// Возвращает все клетки, достижимые из стартовой позиции
    /// за указанное количество шагов.
    ///
    /// Используется для логики отступления и может быть полезен
    /// как основа для будущих более сложных алгоритмов выбора позиции.
    /// </summary>
    public ImmutableHashSet<Position> GetReachableCells(
        BaseUnit actor,
        Position start,
        int maxDistance)
    {
        var result = new HashSet<Position> { start };
        var queue = new Queue<(Position Position, int Distance)>();

        queue.Enqueue((start, 0));

        while (queue.Count > 0)
        {
            (Position current, int distance) = queue.Dequeue();

            if (distance >= maxDistance)
            {
                continue;
            }

            foreach (Position neighbor in GetNeighbors(current))
            {
                if (result.Contains(neighbor))
                {
                    continue;
                }

                if (!MovementRules.CanStandOn(this, actor, neighbor))
                {
                    continue;
                }

                result.Add(neighbor);
                queue.Enqueue((neighbor, distance + 1));
            }
        }

        return [.. result];
    }

    /// <summary>
    /// Возвращает ортогональных соседей для указанной позиции
    /// в пределах арены.
    /// </summary>
    private IEnumerable<Position> GetNeighbors(Position position)
    {
        if (position.X + 1 < Arena.GridWidth)
        {
            yield return new Position(position.X + 1, position.Y);
        }

        if (position.X - 1 >= 0)
        {
            yield return new Position(position.X - 1, position.Y);
        }

        if (position.Y + 1 < Arena.GridHeight)
        {
            yield return new Position(position.X, position.Y + 1);
        }

        if (position.Y - 1 >= 0)
        {
            yield return new Position(position.X, position.Y - 1);
        }
    }
}
