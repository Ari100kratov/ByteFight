using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Contracts.GameRuntime;
using Domain.GameRuntime.GameActionLogs;
using Domain.GameRuntime.GameSessions;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.GameRuntime.GameSessions.GetLogs;

internal sealed class GetGameSessionLogsQueryHandler(IGameRuntimeDbContext dbContext, IUserContext userContext)
    : IQueryHandler<GetGameSessionLogsQuery, IReadOnlyList<TurnLogDto>>
{
    public async Task<Result<IReadOnlyList<TurnLogDto>>> Handle(
        GetGameSessionLogsQuery query,
        CancellationToken cancellationToken)
    {
        bool exists = await dbContext.GameSessions
            .AsNoTracking()
            .AnyAsync(s =>
                s.Id == query.SessionId &&
                s.UserIds.Contains(userContext.UserId),
                cancellationToken);

        if (!exists)
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
