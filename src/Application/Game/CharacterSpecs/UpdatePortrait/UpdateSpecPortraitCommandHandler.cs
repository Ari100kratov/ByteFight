using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Game.CharacterSpecs;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.CharacterSpecs.UpdatePortrait;

internal sealed class UpdateSpecPortraitCommandHandler(IGameDbContext dbContext)
    : ICommandHandler<UpdateSpecPortraitCommand>
{
    public async Task<Result> Handle(UpdateSpecPortraitCommand command, CancellationToken cancellationToken)
    {
        CharacterSpec? characterSpec = await dbContext.CharacterSpecs
            .SingleOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (characterSpec is null)
        {
            return Result.Failure(CharacterSpecErrors.NotFound(command.Id));
        }

        characterSpec.PortraitUrl = command.PortraitUrl.ToString();

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
