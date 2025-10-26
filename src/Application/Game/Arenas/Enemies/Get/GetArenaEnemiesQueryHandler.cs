using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Game.Arenas.Enemies.Dtos;
using Domain.Game.Arenas;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Arenas.Enemies.Get;

internal sealed class GetArenaEnemiesQueryHandler(IGameDbContext dbContext)
    : IQueryHandler<GetArenaEnemiesQuery, IReadOnlyList<ArenaEnemyResponse>>
{
    public async Task<Result<IReadOnlyList<ArenaEnemyResponse>>> Handle(GetArenaEnemiesQuery query, CancellationToken cancellationToken)
    {
        Arena? arena = await dbContext.Arenas
            .SingleOrDefaultAsync(a => a.Id == query.ArenaId, cancellationToken);

        if (arena is null)
        {
            return Result.Failure<IReadOnlyList<ArenaEnemyResponse>>(ArenaErrors.NotFound(query.ArenaId));
        }

        List<ArenaEnemyResponse> enemies = await dbContext.ArenaEnemies
            .AsNoTracking()
            .Where(e => e.ArenaId == query.ArenaId)
            .Include(e => e.Enemy)
            .Select(e => new ArenaEnemyResponse(e.Id, e.EnemyId, e.Enemy.Name, e.Position.ToDto()))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<ArenaEnemyResponse>>(enemies);
    }
}
