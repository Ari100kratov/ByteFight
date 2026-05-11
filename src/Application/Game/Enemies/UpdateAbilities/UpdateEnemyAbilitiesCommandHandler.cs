using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Domain.Game.Enemies;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Enemies.UpdateAbilities;

internal sealed class UpdateEnemyAbilitiesCommandHandler(IGameDbContext dbContext)
    : ICommandHandler<UpdateEnemyAbilitiesCommand>
{
    public async Task<Result> Handle(UpdateEnemyAbilitiesCommand command, CancellationToken cancellationToken)
    {
        Enemy? enemy = await dbContext.Enemies
            .Include(x => x.Abilities)
                .ThenInclude(x => x.Stats)
            .Include(x => x.Abilities)
                .ThenInclude(x => x.ActionAssets)
            .SingleOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (enemy is null)
        {
            return Result.Failure(EnemyErrors.NotFound(command.Id));
        }

        dbContext.EnemyAbilities.RemoveRange(enemy.Abilities);

        var newAbilities = command.Abilities
            .Select(x => x.ToEnemyAbility(enemy.Id))
            .ToList();

        await dbContext.EnemyAbilities.AddRangeAsync(newAbilities, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
