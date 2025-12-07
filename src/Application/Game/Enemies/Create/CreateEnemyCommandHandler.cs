using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Domain.Game.Enemies;
using SharedKernel;

namespace Application.Game.Enemies.Create;

internal sealed class CreateEnemyCommandHandler(IGameDbContext dbContext)
    : ICommandHandler<CreateEnemyCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateEnemyCommand command, CancellationToken cancellationToken)
    {
        var enemyId = Guid.CreateVersion7();

        var enemy = new Enemy
        {
            Id = enemyId,
            Name = command.Name.Trim(),
            Description = command.Description,
            Stats = [.. command.Stats.Select(s => s.ToEnemyStat(enemyId))],
            ActionAssets = [.. command.ActionAssets.Select(a => a.ToEnemyActionAsset(enemyId))]
        };

        dbContext.Enemies.Add(enemy);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(enemy.Id);
    }
}
