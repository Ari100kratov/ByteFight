using Domain;
using Domain.Game.Statuses;
using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.GameRuntime.GameResults;
using Domain.ValueObjects;
using GameRuntime.Common.World.Abilities;
using GameRuntime.Common.World.ArenaItems;
using GameRuntime.Common.World.Statuses;
using GameRuntime.Common.World.Units;
using GameRuntime.Common.World.Stats;
using SharedKernel;

namespace GameRuntime.Common.World;

/// <summary>
/// Runtime-состояние арены: участники, порядок ходов по инициативе,
/// активные статусы, предметы и фабрики записей журнала боя.
/// </summary>
public sealed class ArenaWorld
{
    /// <summary>
    /// Идентификатор runtime-сессии, которым помечаются создаваемые записи журнала.
    /// </summary>
    public Guid GameSessionId { get; } = Guid.CreateVersion7();

    public required ArenaDefinition Arena { get; init; }

    public required PlayerUnit Player { get; init; }

    public required IReadOnlyList<EnemyUnit> Enemies { get; init; }

    /// <summary>
    /// Номер текущего раунда (1 — первый раунд).
    /// </summary>
    public int RoundNumber { get; private set; }

    /// <summary>
    /// Монотонный счётчик ходов для записей журнала.
    /// </summary>
    public int TurnIndex { get; private set; }

    /// <summary>
    /// Юнит, чей ход выполняется прямо сейчас.
    /// </summary>
    public BaseUnit? ActiveUnit { get; private set; }

    /// <summary>
    /// Очередь ходов текущего раунда.
    /// </summary>
    private Queue<BaseUnit> turnQueue = new();

    /// <summary>
    /// Порядок ходов текущего раунда (для отображения клиенту).
    /// </summary>
    public IReadOnlyList<UnitId> RoundOrder =>
        [.. turnQueue.Select(x => new UnitId(x.Id))];

    /// <summary>
    /// Все участники боя.
    /// </summary>
    public IEnumerable<BaseUnit> AllUnits =>
        [Player, .. Enemies];

    /// <summary>
    /// Увеличивает номер текущего хода.
    /// </summary>
    public void IncrementTurn() => TurnIndex++;

    /// <summary>
    /// Начинает новый раунд: увеличивает номер, строит очередь ходов
    /// по инициативе и возвращает запись журнала о начале раунда.
    /// </summary>
    public RoundStartedLogEntry StartNextRound()
    {
        RoundNumber++;
        IncrementTurn();

        var order = AllUnits
            .Where(u => !u.IsDead)
            .OrderByDescending(u => u.Stats.GetInitiative())
            .ThenBy(u => u is PlayerUnit ? 0 : 1) // при равной инициативе игрок ходит первым
            .ToList();

        turnQueue = new Queue<BaseUnit>(order);

        return new RoundStartedLogEntry(
            GameSessionId,
            new UnitId(Player.Id),
            Player.Name,
            $"Начался раунд {RoundNumber}",
            RoundNumber,
            TurnIndex);
    }

    /// <summary>
    /// Возвращает следующего юнита в очереди хода или null, если раунд закончился.
    /// </summary>
    public BaseUnit? AdvanceToNextUnit()
    {
        while (turnQueue.Count > 0)
        {
            BaseUnit unit = turnQueue.Dequeue();

            if (!unit.IsDead)
            {
                ActiveUnit = unit;
                return unit;
            }
        }

        ActiveUnit = null;
        return null;
    }

    /// <summary>
    /// Начинает ход юнита: снимает перезарядки, разрешает периодические
    /// эффекты статусов, восстанавливает ману и задаёт очки действия
    /// и перемещения. Возвращает записи журнала о сработавших эффектах.
    /// Если юнит оглушён, очки обнуляются.
    /// </summary>
    public IReadOnlyList<GameActionLogEntry> BeginUnitTurn(BaseUnit unit)
    {
        var entries = new List<GameActionLogEntry>();
        UnitBattleState state = unit.BattleState;

        state.TickCooldowns();

        // Периодические эффекты срабатывают в начале хода носителя.
        foreach (RuntimeStatusEffect effect in state.Statuses.All)
        {
            switch (effect.Type)
            {
                case StatusEffectType.Burn:
                case StatusEffectType.Poison:
                    StatSnapshot damaged = unit.Stats.ApplyDamage(effect.Magnitude);
                    entries.Add(CreateStatusTickEntry(unit, effect, damaged));
                    break;

                case StatusEffectType.Regeneration:
                    StatApplyResult healed = unit.Stats.Heal(effect.Magnitude);
                    entries.Add(CreateStatusTickEntry(unit, effect, healed.Snapshot));
                    break;
            }
        }

        unit.Stats.RegenerateMana(unit.Stats.GetManaRegen());

        if (unit.IsDead)
        {
            // Погиб от периодического урона.
            unit.MarkKilledBy(unit.Id);
            state.MovePointsRemaining = 0;
            state.ActionsRemaining = 0;
            entries.Add(CreateDeathLogEntry(unit));
            return entries;
        }

        if (state.Statuses.IsStunned)
        {
            state.MovePointsRemaining = 0;
            state.ActionsRemaining = 0;
        }
        else
        {
            int moveRange = unit.Stats.GetMoveRange();
            state.MovePointsRemaining = Math.Max(0, moveRange - state.Statuses.MovePointsPenalty);
            state.ActionsRemaining = UnitBattleState.ActionsPerTurn;
        }

        return entries;
    }

    /// <summary>
    /// Завершает ход юнита: уменьшает длительности активных статусов
    /// и снимает истёкшие.
    /// </summary>
    public void EndUnitTurn(BaseUnit unit) => unit.BattleState.Statuses.TickAndExpire();

    private StatusAppliedLogEntry CreateStatusTickEntry(
        BaseUnit unit,
        RuntimeStatusEffect effect,
        StatSnapshot snapshot) =>
        new StatusAppliedLogEntry(
            GameSessionId,
            new UnitId(unit.Id),
            unit.Name,
            $"{DescribeStatus(effect.Type)} терзает {unit.Name}",
            new UnitId(unit.Id),
            unit.Name,
            effect.Type,
            effect.RemainingTurns,
            effect.Magnitude,
            snapshot,
            TurnIndex);

    /// <summary>
    /// Возвращает человекочитаемое название статуса.
    /// </summary>
    public static string DescribeStatus(StatusEffectType type) => type switch
    {
        StatusEffectType.Burn => "Горение",
        StatusEffectType.Poison => "Яд",
        StatusEffectType.Regeneration => "Регенерация",
        StatusEffectType.Shield => "Щит",
        StatusEffectType.Slow => "Замедление",
        StatusEffectType.Root => "Оковы корней",
        StatusEffectType.Stun => "Оглушение",
        StatusEffectType.Weaken => "Немощь",
        StatusEffectType.Might => "Мощь",
        StatusEffectType.Ward => "Оберег",
        _ => type.ToString()
    };

    /// <summary>
    /// Проверяет, враждуют ли юниты. Юниты разных типов без явных команд
    /// считаются врагами; явно заданные команды сравниваются напрямую.
    /// </summary>
    public bool IsHostile(BaseUnit a, BaseUnit b)
    {
        if (a.TeamId != Guid.Empty && b.TeamId != Guid.Empty)
        {
            return a.TeamId != b.TeamId;
        }

        return a is PlayerUnit != b is PlayerUnit;
    }

    /// <summary>
    /// Является ли юнит союзником игрока.
    /// </summary>
    public bool IsPlayerSide(BaseUnit unit) =>
        unit.TeamId != Guid.Empty
            ? unit.TeamId == Player.TeamId
            : unit is PlayerUnit;

    /// <summary>
    /// Возвращает юнита, стоящего на гексе, или null.
    /// </summary>
    public BaseUnit? GetOccupant(Position position) =>
        AllUnits.FirstOrDefault(u => !u.IsDead && u.Position == position);

    /// <summary>
    /// Занят ли гекс другим (не указанным) живым юнитом.
    /// </summary>
    public bool IsOccupiedByOther(Position position, BaseUnit except) =>
        AllUnits.Any(u => !u.IsDead && u.Id != except.Id && u.Position == position);

    /// <summary>
    /// Возвращает игрока или NPC по runtime-идентификатору юнита.
    /// </summary>
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

    /// <summary>
    /// Проверяет, завершён ли бой, и возвращает результат, если финальное состояние достигнуто.
    /// </summary>
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

        if (RoundNumber >= Arena.MaxTurnsCount)
        {
            return GameResult.TurnLimitLoss();
        }

        return null;
    }

    public IdleLogEntry CreateIdleLogEntry(BaseUnit actor, string? info)
        => new(GameSessionId, new UnitId(actor.Id), actor.Name, info, TurnIndex);

    public WalkLogEntry CreateWalkLogEntry(BaseUnit actor, IReadOnlyList<Position>? path = null)
        => new(GameSessionId, new UnitId(actor.Id), actor.Name, null, actor.FacingDirection, actor.Position, TurnIndex, path);

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

    public StatusAppliedLogEntry CreateStatusAppliedLogEntry(
        BaseUnit actor,
        BaseUnit target,
        StatusEffectType type,
        int duration,
        decimal magnitude,
        StatSnapshot? targetHp)
        => new(
            GameSessionId,
            new UnitId(actor.Id),
            actor.Name,
            null,
            new UnitId(target.Id),
            target.Name,
            type,
            duration,
            magnitude,
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
}
