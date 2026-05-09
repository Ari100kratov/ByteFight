using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Domain.Game.CharacterSpecs;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.CharacterSpecs.UpdateAbilities;

internal sealed class UpdateSpecAbilitiesCommandHandler(IGameDbContext dbContext)
    : ICommandHandler<UpdateSpecAbilitiesCommand>
{
    public async Task<Result> Handle(UpdateSpecAbilitiesCommand command, CancellationToken cancellationToken)
    {
        CharacterSpec? characterSpec = await dbContext.CharacterSpecs
            .Include(x => x.Abilities)
                .ThenInclude(x => x.Stats)
            .Include(x => x.Abilities)
                .ThenInclude(x => x.ActionAssets)
            .SingleOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (characterSpec is null)
        {
            return Result.Failure(CharacterSpecErrors.NotFound(command.Id));
        }

        dbContext.CharacterSpecAbilities.RemoveRange(characterSpec.Abilities);
        characterSpec.Abilities = [.. command.Abilities.Select(x => x.ToCharacterSpecAbility(characterSpec.Id))];

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
