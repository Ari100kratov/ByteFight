using Domain.GameRuntime.GameActionLogs;
using Domain.ValueObjects;
using GameRuntime.Common.World.Abilities;
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

    public virtual Guid Id { get; }

    public required string Name { get; init; }

    public bool IsDead => Stats.IsDead();

    public void Move(Position newPosition)
    {
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
}
