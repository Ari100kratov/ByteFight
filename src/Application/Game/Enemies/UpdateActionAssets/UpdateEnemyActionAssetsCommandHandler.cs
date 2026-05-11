using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Domain.Game.Enemies;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Enemies.UpdateActionAssets;

internal sealed class UpdateEnemyActionAssetsCommandHandler(IGameDbContext dbContext)
    : ICommandHandler<UpdateEnemyActionAssetsCommand>
{
    public async Task<Result> Handle(UpdateEnemyActionAssetsCommand command, CancellationToken cancellationToken)
    {
        Enemy? enemy = await dbContext.Enemies
            .Include(x => x.ActionAssets)
            .SingleOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (enemy is null)
        {
            return Result.Failure(EnemyErrors.NotFound(command.Id));
        }

        dbContext.EnemyActionAssets.RemoveRange(enemy.ActionAssets);

        var newActionAssets = command.ActionAssets
            .Select(x => x.ToEnemyActionAsset(enemy.Id))
            .ToList();

        await dbContext.EnemyActionAssets.AddRangeAsync(newActionAssets, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
