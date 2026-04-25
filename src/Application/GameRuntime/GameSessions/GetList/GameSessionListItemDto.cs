using Domain.Game.GameModes;
using Domain.GameRuntime.GameResults;
using Domain.GameRuntime.GameSessions;

namespace Application.GameRuntime.GameSessions.GetList;

public sealed class GameSessionListItemDto
{
    public required Guid Id { get; init; }
    public required GameModeType Mode { get; init; }
    public required Guid ArenaId { get; init; }
    public required string? ArenaName { get; init; }
    public required string? CharacterName { get; init; }
    public required string? CharacterClassName { get; init; }
    public required string? CharacterSpecName { get; init; }
    public required DateTime StartedAt { get; init; }
    public required DateTime? EndedAt { get; init; }
    public required int TotalTurns { get; init; }
    public required GameStatus Status { get; init; }
    public required GameOutcome? Outcome { get; init; }
}
