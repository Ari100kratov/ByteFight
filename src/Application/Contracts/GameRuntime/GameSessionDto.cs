using Domain.Game.GameModes;
using Domain.GameRuntime.GameSessions;

namespace Application.Contracts.GameRuntime;

public sealed record GameSessionDto
{
    public Guid Id { get; init; }
    public GameModeType Mode { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime? EndedAt { get; init; }
    public int TotalTurns { get; init; }
    public GameStatus Status { get; init; }
    public GameResultDto? Result { get; init; }
}

public static partial class Mapper
{
    public static GameSessionDto ToDto(this GameSession entity)
    {
        return new()
        {
            Id = entity.Id,
            Mode = entity.Mode,
            StartedAt = entity.StartedAt,
            EndedAt = entity.EndedAt,
            TotalTurns = entity.TotalTurns,
            Status = entity.Status,
            Result = entity.Result.ToDto()
        };
    }
}
