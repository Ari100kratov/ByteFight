using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Domain.Game.Enemies;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Enemies.Update;

internal sealed class UpdateEnemyCommandHandler(IGameDbContext dbContext)
    : ICommandHandler<UpdateEnemyCommand>
{
    public async Task<Result> Handle(UpdateEnemyCommand command, CancellationToken cancellationToken)
    {
        Enemy? enemy = await dbContext.Enemies
            .Include(e => e.Stats)
            .Include(e => e.ActionAssets)
            .SingleOrDefaultAsync(e => e.Id == command.Id, cancellationToken);

        if (enemy is null)
        {
            return Result.Failure(EnemyErrors.NotFound(command.Id));
        }

        enemy.Name = command.Name;
        enemy.Description = command.Description;

        dbContext.EnemyStats.RemoveRange(enemy.Stats);
        enemy.Stats = [.. command.Stats.Select(s => s.ToEnemyStat(enemy.Id))];

        dbContext.EnemyActionAssets.RemoveRange(enemy.ActionAssets);
        enemy.ActionAssets = [.. command.ActionAssets.Select(a => a.ToEnemyActionAsset(enemy.Id))];

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
