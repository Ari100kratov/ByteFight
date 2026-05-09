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
            .Where(a => a.IsActive && a.GameModes.Contains(query.Mode))
            .OrderBy(a => a.CreatedAt)
            .Select(a => new ArenaResponse(
                a.Id,
                a.Name,
                new Uri(a.ImageUrl, UriKind.Relative),
                a.GridWidth,
                a.GridHeight,
                a.Description,
                a.Enemies
                    .GroupBy(e => new
                    {
                        e.EnemyId,
                        e.Enemy.Name
                    })
                    .Select(g => new ArenaEnemySummaryResponse(
                        g.Key.EnemyId,
                        g.Key.Name,
                        g.Count()
                    ))
                    .ToList()
            ))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<ArenaResponse>>(arenas);
    }
}
