using Domain.GameRuntime.GameResults;

namespace Application.Contracts.GameRuntime;

public sealed record GameResultDto
{
    public GameOutcome Outcome { get; init; }
    public Guid? WinnerUnitId { get; init; }
}

public static partial class Mapper
{
    public static GameResultDto ToDto(this GameResult? entity)
    {
        if (entity is null)
        {
            return null;
        }

        return new()
        {
            Outcome = entity.Outcome,
            WinnerUnitId = entity.WinnerUnitId?.Value
        };
    }
}

