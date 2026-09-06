using Domain.GameRuntime.GameActionLogs;
using Domain.GameRuntime.GameActionLogs.Entries;

namespace Application.Contracts.GameRuntime;

public sealed record TurnLogDto
{
    public int TurnIndex { get; init; }
    public GameActionLogEntryDto[] Logs { get; init; }
}

public static partial class Mapper
{
    public static object ToDto(this TurnLog entity)
        => new TurnLogDto
        {
            TurnIndex = entity.TurnIndex,
            Logs = [.. entity.Logs.Select(x => x.ToDto())],
        };

    public static GameActionLogEntryDto ToDto(this GameActionLogEntry entity)
        => entity switch
        {
            AbilityUsedLogEntry abilityUsed => abilityUsed.ToDto(),
            WalkLogEntry walk => walk.ToDto(),
            DeathLogEntry death => death.ToDto(),
            IdleLogEntry idle => idle.ToDto(),
            ItemPickedUpLogEntry itemPickedUp => itemPickedUp.ToDto(),
            StatusAppliedLogEntry statusApplied => statusApplied.ToDto(),
            RoundStartedLogEntry roundStarted => roundStarted.ToDto(),
            _ => throw new ArgumentOutOfRangeException(
                nameof(entity),
                $"Unknown RuntimeLogEntry type: {entity.GetType().Name}")
        };

    private static AbilityUsedLogEntryDto ToDto(this AbilityUsedLogEntry e)
        => new()
        {
            Id = e.Id,
            ActorId = e.ActorId.Value,
            ActorName = e.ActorName,
            Info = e.Info,
            TurnIndex = e.TurnIndex,
            CreatedAt = e.CreatedAt,

            AbilityType = e.AbilityType,
            EffectType = e.EffectType,
            AbilityName = e.AbilityName,

            TargetId = e.TargetId.Value,
            TargetName = e.TargetName,
            Value = e.Value,
            FacingDirection = e.FacingDirection,
            TargetHp = new StatSnapshotDto(e.TargetHp.Current, e.TargetHp.Max)
        };

    private static WalkLogEntryDto ToDto(this WalkLogEntry e)
        => new()
        {
            Id = e.Id,
            ActorId = e.ActorId.Value,
            ActorName = e.ActorName,
            Info = e.Info,
            TurnIndex = e.TurnIndex,
            CreatedAt = e.CreatedAt,

            FacingDirection = e.FacingDirection,
            To = e.To.ToDto(),
            Path = e.Path is null ? null : [.. e.Path.Select(p => p.ToDto())]
        };

    private static DeathLogEntryDto ToDto(this DeathLogEntry e)
        => new()
        {
            Id = e.Id,
            ActorId = e.ActorId.Value,
            ActorName = e.ActorName,
            Info = e.Info,
            TurnIndex = e.TurnIndex,
            CreatedAt = e.CreatedAt,
        };

    private static IdleLogEntryDto ToDto(this IdleLogEntry e)
        => new()
        {
            Id = e.Id,
            ActorId = e.ActorId.Value,
            ActorName = e.ActorName,
            Info = e.Info,
            TurnIndex = e.TurnIndex,
            CreatedAt = e.CreatedAt,
        };

    private static ItemPickedUpLogEntryDto ToDto(this ItemPickedUpLogEntry e)
        => new()
        {
            Id = e.Id,
            ActorId = e.ActorId.Value,
            ActorName = e.ActorName,
            Info = e.Info,
            TurnIndex = e.TurnIndex,
            CreatedAt = e.CreatedAt,

            PlacedItemId = e.PlacedItemId,
            ItemId = e.ItemId,
            ItemName = e.ItemName,
            ItemType = e.ItemType,
            Position = e.Position.ToDto(),
            Value = e.Value,
            ActorHp = new StatSnapshotDto(e.ActorHp.Current, e.ActorHp.Max)
        };

    private static StatusAppliedLogEntryDto ToDto(this StatusAppliedLogEntry e)
        => new()
        {
            Id = e.Id,
            ActorId = e.ActorId.Value,
            ActorName = e.ActorName,
            Info = e.Info,
            TurnIndex = e.TurnIndex,
            CreatedAt = e.CreatedAt,

            TargetId = e.TargetId.Value,
            TargetName = e.TargetName,
            StatusType = e.StatusType,
            Duration = e.Duration,
            Magnitude = e.Magnitude,
            TargetHp = e.TargetHp is null
                ? null
                : new StatSnapshotDto(e.TargetHp.Current, e.TargetHp.Max)
        };

    private static RoundStartedLogEntryDto ToDto(this RoundStartedLogEntry e)
        => new()
        {
            Id = e.Id,
            ActorId = e.ActorId.Value,
            ActorName = e.ActorName,
            Info = e.Info,
            TurnIndex = e.TurnIndex,
            CreatedAt = e.CreatedAt,

            RoundNumber = e.RoundNumber
        };
}
