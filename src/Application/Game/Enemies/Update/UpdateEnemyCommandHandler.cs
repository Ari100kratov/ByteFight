using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
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
            .Include(e => e.Assets)
            .SingleOrDefaultAsync(e => e.Id == command.Id, cancellationToken);

        if (enemy is null)
        {
            return Result.Failure(EnemyErrors.NotFound(command.Id));
        }

        enemy.Name = command.Name;
        enemy.Description = command.Description;

        dbContext.EnemyStats.RemoveRange(enemy.Stats);
        enemy.Stats = [.. command.Stats.Select(s => new EnemyStat
        {
            EnemyId = enemy.Id,
            StatType = s.StatType,
            Value = s.Value
        })];

        dbContext.EnemyAssets.RemoveRange(enemy.Assets);
        enemy.Assets = [.. command.Assets.Select(a => new EnemyAsset
        {
            EnemyId = enemy.Id,
            ActionType = a.ActionType,
            Url = a.Url.ToString(),
        })];

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
