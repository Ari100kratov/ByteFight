using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Arenas.GetByMode;

internal sealed class GetArenasByModeQueryHandler(
    IGameDbContext dbContext)
    : IQueryHandler<GetArenasByModeQuery, IReadOnlyList<ArenaResponse>>
{
    public async Task<Result<IReadOnlyList<ArenaResponse>>> Handle(GetArenasByModeQuery query, CancellationToken cancellationToken)
    {
        List<ArenaResponse> arenas = await dbContext.Arenas
            .AsNoTracking()
            .Where(a => a.GameModes.Contains(query.Mode))
            .Select(a => new ArenaResponse(a.Id, a.Name, a.GridWidth, a.GridHeight, a.Description))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<ArenaResponse>>(arenas);
    }
}
