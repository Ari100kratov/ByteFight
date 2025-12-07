using Domain.Game.Actions;
using Domain.GameRuntime.RuntimeLogEntries;

namespace Application.Contracts.GameRuntime;

public sealed record IdleLogEntryDto
{
    public ActionType Type { get; } = ActionType.Idle;
    public Guid ActorId { get; init; }
}

public sealed record WalkLogEntryDto
{
    public ActionType Type { get; } = ActionType.Walk;
    public Guid ActorId { get; init; }
    public FacingDirection FacingDirection { get; init; }
    public PositionDto To { get; init; }
}

public sealed record AttackLogEntryDto
{
    public ActionType Type { get; init; } = ActionType.Attack;
    public Guid ActorId { get; init; }
    public Guid TargetId { get; init; }
    public decimal Damage { get; init; }
    public FacingDirection FacingDirection { get; init; }
    public StatSnapshotDto TargetHp { get; init; }
}

public sealed record DeathLogEntryDto
{
    public ActionType Type { get; init; } = ActionType.Dead;
    public Guid ActorId { get; init; }
}

public sealed record StatSnapshotDto(decimal Current, decimal Max);
