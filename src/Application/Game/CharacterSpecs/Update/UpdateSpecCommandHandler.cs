using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Domain.Game.CharacterClasses;
using Domain.Game.CharacterSpecs;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.CharacterSpecs.Update;

internal sealed class UpdateSpecCommandHandler(IGameDbContext dbContext)
    : ICommandHandler<UpdateSpecCommand>
{
    public async Task<Result> Handle(UpdateSpecCommand command, CancellationToken cancellationToken)
    {
        CharacterSpec? characterSpec = await dbContext.CharacterSpecs
            .Include(e => e.Stats)
            .Include(e => e.ActionAssets)
            .SingleOrDefaultAsync(e => e.Id == command.Id, cancellationToken);

        if (characterSpec is null)
        {
            return Result.Failure(CharacterClassErrors.NotFound(command.Id));
        }

        characterSpec.Name = command.Name;
        characterSpec.Description = command.Description;

        dbContext.CharacterSpecStats.RemoveRange(characterSpec.Stats);
        characterSpec.Stats = [.. command.Stats.Select(s => s.ToCharacterSpecStat(characterSpec.Id))];

        dbContext.CharacterSpecActionAssets.RemoveRange(characterSpec.ActionAssets);
        characterSpec.ActionAssets = [.. command.ActionAssets.Select(a => a.ToCharacterSpecActionAsset(characterSpec.Id))];

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
