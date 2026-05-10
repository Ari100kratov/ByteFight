using System.Text.Json.Serialization;
using Domain.Game.Abilities;
using Domain.Game.ArenaItems;
using Domain.GameRuntime.GameActionLogs;

namespace Application.Contracts.GameRuntime;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "entryType")]
[JsonDerivedType(typeof(IdleLogEntryDto), (int)GameActionLogEntryType.Idle)]
[JsonDerivedType(typeof(WalkLogEntryDto), (int)GameActionLogEntryType.Walk)]
[JsonDerivedType(typeof(AbilityUsedLogEntryDto), (int)GameActionLogEntryType.AbilityUsed)]
[JsonDerivedType(typeof(DeathLogEntryDto), (int)GameActionLogEntryType.Death)]
[JsonDerivedType(typeof(ItemPickedUpLogEntryDto), (int)GameActionLogEntryType.ItemPickedUp)]
public abstract record GameActionLogEntryDto
{
    public required Guid Id { get; init; }
    public required Guid ActorId { get; init; }
    public required string ActorName { get; init; }
    public required string? Info { get; init; }
    public required int TurnIndex { get; init; }
    public required DateTime CreatedAt { get; init; }
}

public sealed record IdleLogEntryDto : GameActionLogEntryDto;

public sealed record WalkLogEntryDto : GameActionLogEntryDto
{
    public required FacingDirection FacingDirection { get; init; }
    public required PositionDto To { get; init; }
}

public sealed record AbilityUsedLogEntryDto : GameActionLogEntryDto
{
    public required AbilityType AbilityType { get; init; }
    public required AbilityEffectType EffectType { get; init; }
    public required string? AbilityName { get; init; }

    public required Guid TargetId { get; init; }
    public required string TargetName { get; init; }

    public required decimal Value { get; init; }

    public required FacingDirection FacingDirection { get; init; }
    public required StatSnapshotDto TargetHp { get; init; }
}

public sealed record DeathLogEntryDto : GameActionLogEntryDto;

public sealed record ItemPickedUpLogEntryDto : GameActionLogEntryDto
{
    public required Guid PlacedItemId { get; init; }
    public required Guid ItemId { get; init; }
    public required string ItemName { get; init; }
    public required ArenaItemType ItemType { get; init; }
    public required PositionDto Position { get; init; }
    public required decimal Value { get; init; }
    public required StatSnapshotDto ActorHp { get; init; }
}

public sealed record StatSnapshotDto(decimal Current, decimal Max);
