using Application.Abstractions.Authorization;
using Application.Abstractions.Data;
using Application.Contracts.GameRuntime;
using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.GameRuntime.GameSessions;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.GameRuntime.GameSessions.GetLogs;

internal sealed class GetGameSessionLogsQueryHandler(
    IGameRuntimeDbContext dbContext,
    IUserAccessService userAccessService)
    : IQueryHandler<GetGameSessionLogsQuery, IReadOnlyList<TurnLogDto>>
{
    public async Task<Result<IReadOnlyList<TurnLogDto>>> Handle(
        GetGameSessionLogsQuery query,
        CancellationToken cancellationToken)
    {
        GameSession? session = await dbContext.GameSessions
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id == query.SessionId, cancellationToken);

        if (session is null ||
            !await userAccessService.CanAccessAnyUserOwnedResourceAsync(session.UserIds, cancellationToken))
        {
            return Result.Failure<IReadOnlyList<TurnLogDto>>(
                GameSessionErrors.NotFound(query.SessionId));
        }

        List<GameActionLogEntry> logs = await dbContext.GameActionLogEntries
            .AsNoTracking()
            .Where(x => x.SessionId == query.SessionId)
            .OrderBy(x => x.TurnIndex)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        var grouped = logs
            .GroupBy(x => x.TurnIndex)
            .OrderBy(g => g.Key)
            .Select(g => new TurnLogDto
            {
                TurnIndex = g.Key,
                Logs = [.. g.Select(x => x.ToDto())]
            })
            .ToList();

        return Result.Success<IReadOnlyList<TurnLogDto>>(grouped);
    }
}
