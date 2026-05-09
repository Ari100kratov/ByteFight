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

    public EnemyBattleAiState AiState { get; } = new();
}

public sealed class EnemyBattleAiState
{
    public bool HasRetreatedAfterBeingHit { get; private set; }

    public bool HasSelfHealedAfterBeingHit { get; private set; }

    public void MarkRetreatedAfterBeingHit()
    {
        HasRetreatedAfterBeingHit = true;
    }

    public void MarkSelfHealedAfterBeingHit()
    {
        HasSelfHealedAfterBeingHit = true;
    }
}
