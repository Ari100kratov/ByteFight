using Domain.Game.Actions;
using Domain.ValueObjects;

namespace Domain.GameRuntime.RuntimeLogEntries;

public abstract record RuntimeLogEntry(Guid ActorId, ActionType Type);

public sealed record AttackLogEntry(
    Guid ActorId,
    Guid TargetId,
    decimal Damage,
    FacingDirection FacingDirection,
    StatSnapshot TargetHp
) : RuntimeLogEntry(ActorId, ActionType.Attack);

public sealed record WalkLogEntry(
    Guid ActorId,
    FacingDirection FacingDirection,
    Position To
) : RuntimeLogEntry(ActorId, ActionType.Walk);

public sealed record DeathLogEntry(Guid ActorId)
    : RuntimeLogEntry(ActorId, ActionType.Dead);

public sealed record IdleLogEntry(Guid ActorId)
    : RuntimeLogEntry(ActorId, ActionType.Idle);

public sealed record StatSnapshot(decimal Current, decimal Max);

public enum FacingDirection
{
    Left = 1,
    Right = 2
}
