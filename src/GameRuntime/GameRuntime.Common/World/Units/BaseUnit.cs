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

    public required string Name { get; init; }

    public virtual Guid Id { get; }

    public bool IsDead => Stats.IsDead();

    public void Move(Position newPosition)
    {
        ThrowIfDead();

        int dx = newPosition.X - Position.X;

        if (dx != 0)
        {
            FacingDirection newFacingDirection = dx > 0
                ? FacingDirection.Right
                : FacingDirection.Left;

            Turn(newFacingDirection);
        }

        Position = newPosition;
    }

    public void Turn(FacingDirection facingDirection)
    {
        ThrowIfDead();
        FacingDirection = facingDirection;
    }

    public void MarkKilledBy(Guid killerId)
    {
        if (KilledByUnitId is not null)
        {
            return;
        }

        KilledByUnitId = killerId;
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
