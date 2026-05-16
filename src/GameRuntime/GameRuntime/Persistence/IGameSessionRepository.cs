using Application.Abstractions.GameRuntime;
using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.GameRuntime.GameResults;
using Domain.GameRuntime.GameSessions;

namespace GameRuntime.Persistence;

internal interface IGameSessionRepository
{
    Task<GameSession> Create(
        Guid id,
        GameInitModel initModel,
        string characterName,
        IEnumerable<GameSessionParticipantInitModel> arenaEnemies,
        CancellationToken ct);

    Task Save(IEnumerable<GameActionLogEntry> gameActionLogEntries);

    Task<GameSession> MarkStarted(Guid id);

    Task<GameSession> CompleteSuccess(Guid id, GameResult gameResult, int turns);

    Task<GameSession> CompleteWithError(Guid id, string? reason, int turns);

    Task<GameSession> Abort(Guid id, int turns);
}

internal sealed record GameSessionParticipantInitModel(Guid UnitId, string Name);

internal sealed record PlayerParticipantMetadata(
    string? UserFirstName,
    string? UserLastName,
    string? CharacterClassName,
    string? CharacterSpecName);
