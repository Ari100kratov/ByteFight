using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Game.Enemies;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Enemies.Rename;

internal sealed class RenameEnemyCommandHandler(IGameDbContext dbContext)
    : ICommandHandler<RenameEnemyCommand>
{
    public async Task<Result> Handle(RenameEnemyCommand command, CancellationToken cancellationToken)
    {
        Enemy? enemy = await dbContext.Enemies
            .SingleOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (enemy is null)
        {
            return Result.Failure(EnemyErrors.NotFound(command.Id));
        }

        enemy.Name = command.Name;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
