using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Contracts.GameRuntime;
using Domain.GameRuntime.GameSessions;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.GameRuntime.GameSessions.GetById;

internal sealed class GetGameSessionByIdQueryHandler(IGameRuntimeDbContext dbContext, IUserContext userContext)
    : IQueryHandler<GetGameSessionByIdQuery, GameSessionDto>
{
    public async Task<Result<GameSessionDto>> Handle(GetGameSessionByIdQuery query, CancellationToken cancellationToken)
    {
        GameSessionDto? session = await dbContext.GameSessions
            .AsNoTracking()
            .Where(c => c.Id == query.Id && c.UserIds.Contains(userContext.UserId))
            .Select(c => c.ToDto())
            .SingleOrDefaultAsync(cancellationToken);

        if (session is null)
        {
            return Result.Failure<GameSessionDto>(GameSessionErrors.NotFound(query.Id));
        }

        return Result.Success(session);
    }
}
