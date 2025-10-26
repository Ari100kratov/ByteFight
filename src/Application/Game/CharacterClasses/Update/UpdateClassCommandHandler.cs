using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Game.Common.Dtos;
using Domain.Game.CharacterClasses;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.CharacterClasses.Update;

internal sealed class UpdateClassCommandHandler(IGameDbContext dbContext)
    : ICommandHandler<UpdateClassCommand>
{
    public async Task<Result> Handle(UpdateClassCommand command, CancellationToken cancellationToken)
    {
        CharacterClass? characterClass = await dbContext.CharacterClasses
            .Include(e => e.Stats)
            .Include(e => e.ActionAssets)
            .SingleOrDefaultAsync(e => e.Id == command.Id, cancellationToken);

        if (characterClass is null)
        {
            return Result.Failure(CharacterClassErrors.NotFound(command.Id));
        }

        characterClass.Name = command.Name;
        characterClass.Description = command.Description;

        dbContext.CharacterClassStats.RemoveRange(characterClass.Stats);
        characterClass.Stats = [.. command.Stats.Select(s => s.ToCharacterClassStat(characterClass.Id))];

        dbContext.CharacterClassActionAssets.RemoveRange(characterClass.ActionAssets);
        characterClass.ActionAssets = [.. command.ActionAssets.Select(a => a.ToCharacterClassActionAsset(characterClass.Id))];

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
