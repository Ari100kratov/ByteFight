using Application.Abstractions.Data;

using Domain.Game.CharacterSpecs;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.Game.CharacterSpecs.Rename;

internal sealed class RenameSpecCommandHandler(IGameDbContext dbContext)
    : ICommandHandler<RenameSpecCommand>
{
    public async Task<Result> Handle(RenameSpecCommand command, CancellationToken cancellationToken)
    {
        CharacterSpec? characterSpec = await dbContext.CharacterSpecs
            .SingleOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (characterSpec is null)
        {
            return Result.Failure(CharacterSpecErrors.NotFound(command.Id));
        }

        characterSpec.Name = command.Name;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
