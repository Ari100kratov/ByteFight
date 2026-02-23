using Domain.GameRuntime.GameActionLogs;

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
            AttackLogEntry attack => attack.ToDto(),
            WalkLogEntry walk => walk.ToDto(),
            DeathLogEntry death => death.ToDto(),
            IdleLogEntry idle => idle.ToDto(),
            _ => throw new ArgumentOutOfRangeException(nameof(entity), $"Unknown RuntimeLogEntry type: {entity.GetType().Name}")
        };

    private static AttackLogEntryDto ToDto(this AttackLogEntry e)
        => new()
        {
            Id = e.Id,
            ActorId = e.ActorId,
            Info = e.Info,
            TurnIndex = e.TurnIndex,
            CreatedAt = e.CreatedAt,

            TargetId = e.TargetId,
            Damage = e.Damage,
            FacingDirection = e.FacingDirection,
            TargetHp = new StatSnapshotDto(e.TargetHp.Current, e.TargetHp.Max)
        };

    private static WalkLogEntryDto ToDto(this WalkLogEntry e)
        => new()
        {
            Id = e.Id,
            ActorId = e.ActorId,
            Info = e.Info,
            TurnIndex = e.TurnIndex,
            CreatedAt = e.CreatedAt,

            FacingDirection = e.FacingDirection,
            To = e.To.ToDto()
        };

    private static DeathLogEntryDto ToDto(this DeathLogEntry e)
        => new()
        {
            Id = e.Id,
            ActorId = e.ActorId,
            Info = e.Info,
            TurnIndex = e.TurnIndex,
            CreatedAt = e.CreatedAt,
        };

    private static IdleLogEntryDto ToDto(this IdleLogEntry e)
        => new()
        {
            Id = e.Id,
            ActorId = e.ActorId,
            Info = e.Info,
            TurnIndex = e.TurnIndex,
            CreatedAt = e.CreatedAt,
        };
}
