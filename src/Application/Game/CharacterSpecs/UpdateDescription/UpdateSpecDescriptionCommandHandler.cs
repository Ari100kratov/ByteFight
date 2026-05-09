using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Game.CharacterSpecs;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.CharacterSpecs.UpdateDescription;

internal sealed class UpdateSpecDescriptionCommandHandler(IGameDbContext dbContext)
    : ICommandHandler<UpdateSpecDescriptionCommand>
{
    public async Task<Result> Handle(UpdateSpecDescriptionCommand command, CancellationToken cancellationToken)
    {
        CharacterSpec? characterSpec = await dbContext.CharacterSpecs
            .SingleOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (characterSpec is null)
        {
            return Result.Failure(CharacterSpecErrors.NotFound(command.Id));
        }

        characterSpec.Description = command.Description;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
