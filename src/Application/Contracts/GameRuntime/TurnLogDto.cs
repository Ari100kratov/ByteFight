using Domain.GameRuntime.RuntimeLogEntries;

namespace Application.Contracts.GameRuntime;

public sealed record TurnLogDto
{
    public int TurnIndex { get; init; }
    public object[] Logs { get; init; } // TODO: object? Сомнительно, но окей...
}

public static partial class Mapper
{
    public static object ToDto(this TurnLog entity)
        => new TurnLogDto
        {
            TurnIndex = entity.TurnIndex,
            Logs = [.. entity.Logs.Select(x => x.ToDto())],
        };

    private static object ToDto(this RuntimeLogEntry entity)
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
            ActorId = e.ActorId,
            TargetId = e.TargetId,
            Damage = e.Damage,
            FacingDirection = e.FacingDirection,
            TargetHp = new StatSnapshotDto(e.TargetHp.Current, e.TargetHp.Max)
        };

    private static WalkLogEntryDto ToDto(this WalkLogEntry e)
        => new()
        {
            ActorId = e.ActorId,
            FacingDirection = e.FacingDirection,
            To = e.To.ToDto()
        };

    private static DeathLogEntryDto ToDto(this DeathLogEntry e)
        => new() { ActorId = e.ActorId };

    private static IdleLogEntryDto ToDto(this IdleLogEntry e)
        => new() { ActorId = e.ActorId };
}
