using System.Linq.Expressions;
using Domain.Game.GameModes;
using Domain.GameRuntime.GameSessionParticipants;
using Domain.GameRuntime.GameSessions;

namespace Application.Contracts.GameRuntime;

public sealed record GameSessionDto
{
    public required Guid Id { get; init; }
    public required Guid CharacterId { get; init; }
    public required GameModeType Mode { get; init; }
    public required DateTime StartedAt { get; init; }
    public required DateTime? EndedAt { get; init; }
    public required int TotalTurns { get; init; }
    public required GameStatus Status { get; init; }
    public required string? ErrorMessage { get; init; }
    public required GameResultDto? Result { get; init; }
}

public static partial class Mapper
{
    public static GameSessionDto ToDto(this GameSession entity)
    {
        return new()
        {
            Id = entity.Id,
            // TODO: сломается, когда будет реализован PVP-режим
            CharacterId = entity.Participants.First(x => x.UnitType is ParticipantUnitType.Player).UnitId.Value,
            Mode = entity.Mode,
            StartedAt = entity.StartedAt,
            EndedAt = entity.EndedAt,
            TotalTurns = entity.TotalTurns,
            Status = entity.Status,
            ErrorMessage = entity.ErrorMessage,
            Result = entity.Result.ToDto()
        };
    }

    public static Expression<Func<GameSession, GameSessionDto>> ToDto()
    {
        return entity => new GameSessionDto
        {
            Id = entity.Id,
            // TODO: сломается, когда будет реализован PVP-режим
            CharacterId = entity.Participants.First(x => x.UnitType == ParticipantUnitType.Player).UnitId.Value,
            Mode = entity.Mode,
            StartedAt = entity.StartedAt,
            EndedAt = entity.EndedAt,
            TotalTurns = entity.TotalTurns,
            Status = entity.Status,
            ErrorMessage = entity.ErrorMessage,
            Result = entity.Result.ToDto()
        };
    }
}
