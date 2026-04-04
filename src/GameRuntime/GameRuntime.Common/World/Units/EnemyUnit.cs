using Domain.GameRuntime.GameActionLogs;
using Domain.ValueObjects;

namespace GameRuntime.Common.World.Units;

public sealed record EnemyUnit : BaseUnit
{
    public EnemyUnit(Position position, FacingDirection facingDirection)
        : base(position, facingDirection) { }

    public override Guid Id => ArenaEnemyId;

    public required Guid ArenaEnemyId { get; init; }

    public required Guid EnemyId { get; init; }
}
