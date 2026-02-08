using Domain.Game.Stats;
using Domain.GameRuntime.RuntimeLogEntries;
using Domain.ValueObjects;
using GameRuntime.Core.Stats;

namespace GameRuntime.Core.Units;

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

    public required RuntimeStats Stats { get; init; }

    public virtual Guid Id { get; }

    public bool IsDead => Stats.Current[StatType.Health] <= 0;

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
