using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Domain.Game.GameModes;
using Domain.GameRuntime.GameResults;
using Domain.GameRuntime.GameSessionParticipants;
using Domain.GameRuntime.GameSessions;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.GameRuntime.GameSessions.GetList;

internal sealed class GetGameSessionsQueryHandler(
    IGameRuntimeDbContext gameRuntimeDbContext,
    IGameDbContext gameDbContext,
    IUserContext userContext)
    : IQueryHandler<GetGameSessionsQuery, PagedResponse<GameSessionListItemDto>>
{
    public async Task<Result<PagedResponse<GameSessionListItemDto>>> Handle(
        GetGameSessionsQuery query,
        CancellationToken cancellationToken)
    {
        int page = query.Page < 1 ? 1 : query.Page;
        int pageSize = query.PageSize switch
        {
            <= 0 => 20,
            > 100 => 100,
            _ => query.PageSize
        };

        IQueryable<GameSession> baseQuery = gameRuntimeDbContext.GameSessions
            .AsNoTracking()
            .Where(x => x.UserIds.Contains(userContext.UserId));

        int totalCount = await baseQuery.CountAsync(cancellationToken);

        List<SessionListProjection> sessions = await baseQuery
            .OrderByDescending(x => x.StartedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new SessionListProjection(
                x.Id,
                x.Mode,
                x.ArenaId.Value,
                x.StartedAt,
                x.EndedAt,
                x.TotalTurns,
                x.Status,
                x.Result != null ? x.Result.Outcome : null,
                x.Participants
                    .Where(p => p.UnitType == ParticipantUnitType.Player)
                    .Select(p => (Guid?)p.UnitId.Value)
                    .FirstOrDefault()))
            .ToListAsync(cancellationToken);

        Guid[] characterIds =
        [
            .. sessions
                .Where(x => x.PlayerCharacterId.HasValue)
                .Select(x => x.PlayerCharacterId!.Value)
                .Distinct()
        ];

        Guid[] arenaIds =
        [
            .. sessions
                .Select(x => x.ArenaId)
                .Distinct()
        ];

        Dictionary<Guid, CharacterLookupItem> characters = characterIds.Length == 0
            ? []
            : await gameDbContext.Characters
                .AsNoTracking()
                .Where(x => characterIds.Contains(x.Id))
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    ClassName = x.Spec.Class.Name,
                    SpecName = x.Spec.Name
                })
                .ToDictionaryAsync(
                    x => x.Id,
                    x => new CharacterLookupItem(x.Name, x.ClassName, x.SpecName),
                    cancellationToken);

        Dictionary<Guid, string> arenas = arenaIds.Length == 0
            ? []
            : await gameDbContext.Arenas
                .AsNoTracking()
                .Where(x => arenaIds.Contains(x.Id))
                .Select(x => new
                {
                    x.Id,
                    x.Name
                })
                .ToDictionaryAsync(
                    x => x.Id,
                    x => x.Name,
                    cancellationToken);

        var items = sessions
            .Select(session =>
            {
                CharacterLookupItem? character = session.PlayerCharacterId.HasValue &&
                                                 characters.TryGetValue(session.PlayerCharacterId.Value, out CharacterLookupItem? foundCharacter)
                    ? foundCharacter
                    : null;

                arenas.TryGetValue(session.ArenaId, out string? arenaName);

                return new GameSessionListItemDto
                {
                    Id = session.Id,
                    Mode = session.Mode,
                    ArenaId = session.ArenaId,
                    ArenaName = arenaName,
                    StartedAt = session.StartedAt,
                    EndedAt = session.EndedAt,
                    TotalTurns = session.TotalTurns,
                    Status = session.Status,
                    Outcome = session.Outcome,
                    CharacterName = character?.Name,
                    CharacterClassName = character?.ClassName,
                    CharacterSpecName = character?.SpecName,
                };
            })
            .ToList();

        return Result.Success(new PagedResponse<GameSessionListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    private sealed record SessionListProjection(
        Guid Id,
        GameModeType Mode,
        Guid ArenaId,
        DateTime StartedAt,
        DateTime? EndedAt,
        int TotalTurns,
        GameStatus Status,
        GameOutcome? Outcome,
        Guid? PlayerCharacterId);

    private sealed record CharacterLookupItem(
        string Name,
        string? ClassName,
        string? SpecName);
}
