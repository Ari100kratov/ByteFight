using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Domain.Game.Enemies;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Enemies.UpdateStats;

internal sealed class UpdateEnemyStatsCommandHandler(IGameDbContext dbContext)
    : ICommandHandler<UpdateEnemyStatsCommand>
{
    public async Task<Result> Handle(UpdateEnemyStatsCommand command, CancellationToken cancellationToken)
    {
        Enemy? enemy = await dbContext.Enemies
            .Include(x => x.Stats)
            .SingleOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (enemy is null)
        {
            return Result.Failure(EnemyErrors.NotFound(command.Id));
        }

        dbContext.EnemyStats.RemoveRange(enemy.Stats);
        enemy.Stats = [.. command.Stats.Select(x => x.ToEnemyStat(enemy.Id))];

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
