using System.Text.Json.Serialization;
using Domain.Game.Actions;
using Domain.GameRuntime.GameActionLogs;

namespace Application.Contracts.GameRuntime;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "actionType")]
[JsonDerivedType(typeof(AttackLogEntryDto), (int)ActionType.Attack)]
[JsonDerivedType(typeof(WalkLogEntryDto), (int)ActionType.Walk)]
[JsonDerivedType(typeof(DeathLogEntryDto), (int)ActionType.Dead)]
[JsonDerivedType(typeof(IdleLogEntryDto), (int)ActionType.Idle)]
public abstract record GameActionLogEntryDto
{
    public required Guid Id { get; init; }
    public required Guid ActorId { get; init; }
    public required string? Info { get; init; }
    public required int TurnIndex { get; init; }
    public required DateTime CreatedAt { get; init; }
}

public sealed record IdleLogEntryDto : GameActionLogEntryDto
{
}

public sealed record WalkLogEntryDto : GameActionLogEntryDto
{
    public required FacingDirection FacingDirection { get; init; }
    public required PositionDto To { get; init; } = default!;
}

public sealed record AttackLogEntryDto : GameActionLogEntryDto
{
    public required Guid TargetId { get; init; }
    public required decimal Damage { get; init; }
    public required FacingDirection FacingDirection { get; init; }
    public required StatSnapshotDto TargetHp { get; init; }
}

public sealed record DeathLogEntryDto : GameActionLogEntryDto
{
}

public sealed record StatSnapshotDto(decimal Current, decimal Max);
