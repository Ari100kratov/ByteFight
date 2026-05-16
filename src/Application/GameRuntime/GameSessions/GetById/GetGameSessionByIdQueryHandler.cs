using Application.Abstractions.Authorization;
using Application.Abstractions.Data;
using Application.Contracts.GameRuntime;
using Domain.GameRuntime.GameSessions;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.GameRuntime.GameSessions.GetById;

internal sealed class GetGameSessionByIdQueryHandler(
    IGameRuntimeDbContext dbContext,
    IUserAccessService userAccessService)
    : IQueryHandler<GetGameSessionByIdQuery, GameSessionDto>
{
    public async Task<Result<GameSessionDto>> Handle(GetGameSessionByIdQuery query, CancellationToken cancellationToken)
    {
        GameSession? session = await dbContext.GameSessions
            .AsNoTracking()
            .Include(x => x.Participants)
            .SingleOrDefaultAsync(c => c.Id == query.Id, cancellationToken);

        if (session is null)
        {
            return Result.Failure<GameSessionDto>(GameSessionErrors.NotFound(query.Id));
        }

        if (!await userAccessService.CanAccessAnyUserOwnedResourceAsync(session.UserIds, cancellationToken))
        {
            return Result.Failure<GameSessionDto>(GameSessionErrors.NotFound(query.Id));
        }

        return Result.Success(session.ToDto());
    }
}
