using Application.Abstractions.Data;

using Domain.Game.Enemies;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.Game.Enemies.UpdateDescription;

internal sealed class UpdateEnemyDescriptionCommandHandler(IGameDbContext dbContext)
    : ICommandHandler<UpdateEnemyDescriptionCommand>
{
    public async Task<Result> Handle(UpdateEnemyDescriptionCommand command, CancellationToken cancellationToken)
    {
        Enemy? enemy = await dbContext.Enemies
            .SingleOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (enemy is null)
        {
            return Result.Failure(EnemyErrors.NotFound(command.Id));
        }

        enemy.Description = command.Description;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
