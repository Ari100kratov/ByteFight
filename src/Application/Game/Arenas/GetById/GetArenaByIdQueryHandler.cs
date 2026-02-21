using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Domain.Game.Arenas;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Arenas.GetById;

internal sealed class GetArenaByIdQueryHandler(IGameDbContext dbContext)
    : IQueryHandler<GetArenaByIdQuery, ArenaResponse>
{
    public async Task<Result<ArenaResponse>> Handle(GetArenaByIdQuery query, CancellationToken cancellationToken)
    {
        ArenaResponse? arena = await dbContext.Arenas
            .AsNoTracking()
            .Where(a => a.Id == query.Id)
            .Select(a => new ArenaResponse(
                a.Id, a.Name,
                a.GridWidth,
                a.GridHeight,
                a.BackgroundAsset,
                a.Description,
                a.StartPosition.ToDto(),
                a.BlockedPositions.Select(x => x.ToDto()).ToArray()
                ))
            .SingleOrDefaultAsync(cancellationToken);

        if (arena is null)
        {
            return Result.Failure<ArenaResponse>(ArenaErrors.NotFound(query.Id));
        }

        return Result.Success(arena);
    }
}
