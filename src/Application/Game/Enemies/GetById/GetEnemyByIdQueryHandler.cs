using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Game.Common.Dtos;
using Domain.Game.Enemies;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Enemies.GetById;

internal sealed class GetEnemyByIdQueryHandler(IGameDbContext dbContext)
    : IQueryHandler<GetEnemyByIdQuery, EnemyResponse>
{
    public async Task<Result<EnemyResponse>> Handle(GetEnemyByIdQuery query, CancellationToken cancellationToken)
    {
        Enemy? enemy = await dbContext.Enemies
            .Include(e => e.Stats)
            .Include(e => e.ActionAssets)
            .SingleOrDefaultAsync(e => e.Id == query.Id, cancellationToken);

        if (enemy is null)
        {
            return Result.Failure<EnemyResponse>(EnemyErrors.NotFound(query.Id));
        }

        var response = new EnemyResponse(
            enemy.Id,
            enemy.Name,
            enemy.Description,
            [.. enemy.Stats.Select(s => s.ToDto())],
            [.. enemy.ActionAssets.Select(a => a.ToDto())]
        );

        return Result.Success(response);
    }
}
