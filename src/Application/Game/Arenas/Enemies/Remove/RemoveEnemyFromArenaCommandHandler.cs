using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Game.Arenas;
using Domain.Game.Arenas.ArenaEnemies;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Arenas.Enemies.Remove;

internal sealed class RemoveEnemyFromArenaCommandHandler(IGameDbContext dbContext)
    : ICommandHandler<RemoveEnemyFromArenaCommand>
{
    public async Task<Result> Handle(RemoveEnemyFromArenaCommand command, CancellationToken cancellationToken)
    {
        Arena? arena = await dbContext.Arenas
            .SingleOrDefaultAsync(a => a.Id == command.ArenaId, cancellationToken);

        if (arena is null)
        {
            return Result.Failure(ArenaErrors.NotFound(command.ArenaId));
        }

        ArenaEnemy? arenaEnemy = await dbContext.ArenaEnemies
            .SingleOrDefaultAsync(e => e.Id == command.ArenaEnemyId && e.ArenaId == command.ArenaId, cancellationToken);

        if (arenaEnemy is null)
        {
            return Result.Failure(ArenaEnemiesErrors.NotFound(command.ArenaEnemyId));
        }

        dbContext.ArenaEnemies.Remove(arenaEnemy);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
