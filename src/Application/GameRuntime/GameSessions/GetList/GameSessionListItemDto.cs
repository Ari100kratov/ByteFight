using Domain.Game.GameModes;
using Domain.GameRuntime.GameResults;
using Domain.GameRuntime.GameSessions;

namespace Application.GameRuntime.GameSessions.GetList;

public sealed class GameSessionListItemDto
{
    public Guid Id { get; init; }

    public GameModeType Mode { get; init; }
    public Guid ArenaId { get; init; }
    public string? ArenaName { get; init; }

    public string? CharacterName { get; set; }
    public string? CharacterClass { get; set; }

    public DateTime StartedAt { get; init; }
    public DateTime? EndedAt { get; init; }

    public int TotalTurns { get; init; }
    public GameStatus Status { get; init; }

    public GameOutcome? Outcome { get; init; }
}
