using Application.Abstractions.Data;

using Application.Contracts;
using Domain.Game.CharacterSpecs;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.Game.CharacterSpecs.UpdateActionAssets;

internal sealed class UpdateSpecActionAssetsCommandHandler(IGameDbContext dbContext)
    : ICommandHandler<UpdateSpecActionAssetsCommand>
{
    public async Task<Result> Handle(UpdateSpecActionAssetsCommand command, CancellationToken cancellationToken)
    {
        CharacterSpec? characterSpec = await dbContext.CharacterSpecs
            .Include(x => x.ActionAssets)
            .SingleOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (characterSpec is null)
        {
            return Result.Failure(CharacterSpecErrors.NotFound(command.Id));
        }

        dbContext.CharacterSpecActionAssets.RemoveRange(characterSpec.ActionAssets);

        var newActionAssets = command.ActionAssets
            .Select(x => x.ToCharacterSpecActionAsset(characterSpec.Id))
            .ToList();

        await dbContext.CharacterSpecActionAssets.AddRangeAsync(newActionAssets, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
