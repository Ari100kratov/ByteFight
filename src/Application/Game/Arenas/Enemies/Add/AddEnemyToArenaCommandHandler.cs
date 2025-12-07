using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Domain.Game.Arenas;
using Domain.Game.Arenas.ArenaEnemies;
using Domain.Game.Enemies;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Arenas.Enemies.Add;

internal sealed class AddEnemyToArenaCommandHandler(
    IGameDbContext dbContext)
    : ICommandHandler<AddEnemyToArenaCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddEnemyToArenaCommand command, CancellationToken cancellationToken)
    {
        Arena? arena = await dbContext.Arenas
            .SingleOrDefaultAsync(a => a.Id == command.ArenaId, cancellationToken);

        if (arena is null)
        {
            return Result.Failure<Guid>(ArenaErrors.NotFound(command.ArenaId));
        }

        Position position = command.Position.ToValueObject();
        if (!position.IsWithinGrid(arena.GridWidth, arena.GridHeight))
        {
            return Result.Failure<Guid>(ArenaEnemiesErrors.InvalidCoordinates(position.X, position.Y));
        }

        Enemy? enemy = await dbContext.Enemies
            .SingleOrDefaultAsync(a => a.Id == command.EnemyId, cancellationToken);

        if (enemy is null)
        {
            return Result.Failure<Guid>(EnemyErrors.NotFound(command.EnemyId));
        }

        bool occupied = await dbContext.ArenaEnemies
            .AnyAsync(e => e.ArenaId == command.ArenaId
                && e.Position.X == position.X
                && e.Position.Y == position.Y,
            cancellationToken);

        if (occupied)
        {
            return Result.Failure<Guid>(ArenaEnemiesErrors.CellOccupied(position.X, position.Y));
        }

        var arenaEnemy = new ArenaEnemy
        {
            Id = Guid.CreateVersion7(),
            ArenaId = command.ArenaId,
            EnemyId = command.EnemyId,
            Position = position,
        };

        dbContext.ArenaEnemies.Add(arenaEnemy);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(arenaEnemy.Id);
    }
}
