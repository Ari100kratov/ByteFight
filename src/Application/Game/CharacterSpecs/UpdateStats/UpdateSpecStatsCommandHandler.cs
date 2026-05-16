using Application.Abstractions.Data;

using Application.Contracts;
using Domain.Game.CharacterSpecs;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.Game.CharacterSpecs.UpdateStats;

internal sealed class UpdateSpecStatsCommandHandler(IGameDbContext dbContext)
    : ICommandHandler<UpdateSpecStatsCommand>
{
    public async Task<Result> Handle(UpdateSpecStatsCommand command, CancellationToken cancellationToken)
    {
        CharacterSpec? characterSpec = await dbContext.CharacterSpecs
            .Include(x => x.Stats)
            .SingleOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (characterSpec is null)
        {
            return Result.Failure(CharacterSpecErrors.NotFound(command.Id));
        }

        dbContext.CharacterSpecStats.RemoveRange(characterSpec.Stats);

        var newStats = command.Stats
            .Select(x => x.ToCharacterSpecStat(characterSpec.Id))
            .ToList();

        await dbContext.CharacterSpecStats.AddRangeAsync(newStats, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
