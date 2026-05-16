using Application.Abstractions.Data;

using Domain.Game.CharacterClasses;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.Game.CharacterClasses.Update;

internal sealed class UpdateClassCommandHandler(IGameDbContext dbContext)
    : ICommandHandler<UpdateClassCommand>
{
    public async Task<Result> Handle(UpdateClassCommand command, CancellationToken cancellationToken)
    {
        CharacterClass? characterClass = await dbContext.CharacterClasses
            .SingleOrDefaultAsync(e => e.Id == command.Id, cancellationToken);

        if (characterClass is null)
        {
            return Result.Failure(CharacterClassErrors.NotFound(command.Id));
        }

        characterClass.Name = command.Name;
        characterClass.Description = command.Description;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
