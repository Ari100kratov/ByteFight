using Domain.Game.Arenas;
using Domain.Game.ArenaItems;
using Domain.GameRuntime.GameActionLogs;
using Domain.ValueObjects;
using GameRuntime.Common.World.Abilities;
using GameRuntime.Common.World.ArenaItems;
using GameRuntime.Common.World.Stats;

namespace GameRuntime.Common.World.Units;

public record BaseUnit
{
    public BaseUnit(Position position, FacingDirection facingDirection)
    {
        Position = position;
        FacingDirection = facingDirection;
        BattleState = new UnitBattleState();
    }

    public Position Position { get; private set; }

    public FacingDirection FacingDirection { get; private set; }

    /// <summary>
    /// Идентификатор юнита, который нанёс смертельный удар.
    /// </summary>
    public Guid? KilledByUnitId { get; private set; }

    /// <summary>
    /// Характеристики юнита во время боя.
    /// </summary>
    public required RuntimeStats Stats { get; init; }

    /// <summary>
    /// Способности юнита, доступные во время боя.
    /// </summary>
    public required RuntimeAbilities Abilities { get; init; }

    /// <summary>
    /// Состояние юнита в текущем бою: очки действия, перезарядки, статусы.
    /// </summary>
    public UnitBattleState BattleState { get; }

    public required string Name { get; init; }

    /// <summary>
    /// Идентификатор команды. Основа для будущих PvP-режимов:
    /// сейчас игрок — одна команда, враги арены — другая.
    /// </summary>
    public Guid TeamId { get; init; } = Guid.Empty;

    public virtual Guid Id { get; }

    public bool IsDead => Stats.IsDead();

    /// <summary>
    /// Перемещает юнита в соседний гекс, обновляя направление взгляда
    /// по направлению шага.
    /// </summary>
    public void MoveStep(Position step)
    {
        ThrowIfDead();

        if (step != Position)
        {
            Turn(Position.CalculateFacing(step));
            Position = step;
        }
    }

    /// <summary>
    /// Поворачивает юнита в указанном направлении.
    /// </summary>
    public void Turn(FacingDirection facingDirection)
    {
        ThrowIfDead();
        FacingDirection = facingDirection;
    }

    /// <summary>
    /// Поворачивает юнита лицом к цели.
    /// </summary>
    public void FaceTarget(Position target)
    {
        Turn(Position.CalculateFacing(target));
    }

    public void MarkKilledBy(Guid killerId)
    {
        if (KilledByUnitId is not null)
        {
            return;
        }

        KilledByUnitId = killerId;
        BattleState.Statuses.Clear();
    }

    /// <summary>
    /// Применяет эффект предмета к юниту.
    /// </summary>
    public StatApplyResult ApplyItem(ArenaItemDefinition item)
    {
        ThrowIfDead();

        return item.Type switch
        {
            ArenaItemType.HealingPotion => Stats.Heal(item.Value),

            _ => throw new NotImplementedException(
                $"Item type '{item.Type}' is not supported yet.")
        };
    }

    /// <summary>
    /// Возвращает множитель входящего урона с учётом рельефа под юнитом.
    /// </summary>
    public decimal GetTerrainDamageMultiplier(IReadOnlyDictionary<Position, TerrainType> terrain)
    {
        TerrainType cell = terrain.GetValueOrDefault(Position, TerrainType.Meadow);

        return TerrainRules.IncomingDamageMultiplier(cell);
    }

    private void ThrowIfDead()
    {
        if (!IsDead)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Unit '{Name}' ({Id}) is dead and cannot perform this action.");
    }
}
