using Domain.GameRuntime.RuntimeLogEntries;
using Domain.ValueObjects;

namespace GameRuntime.World.Units;

internal sealed record EnemyUnit : BaseUnit
{
    public EnemyUnit(Position position, FacingDirection facingDirection)
        : base(position, facingDirection) { }

    public override Guid Id => ArenaEnemyId;

    public required Guid ArenaEnemyId { get; init; }

    public required Guid EnemyId { get; init; }
}
