using Application.Abstractions.Authorization;
using Application.Abstractions.Data;
using Domain.Game.Characters;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.Game.Characters.Rename;

internal sealed class RenameCharacterCommandHandler(
    IGameDbContext dbContext,
    IUserAccessService userAccessService,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<RenameCharacterCommand>
{
    public async Task<Result> Handle(RenameCharacterCommand command, CancellationToken cancellationToken)
    {
        string name = command.Name.Trim();

        Character? character = await dbContext.Characters
            .SingleOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (character is null)
        {
            return Result.Failure(CharacterErrors.NotFound(command.Id));
        }

        if (!await userAccessService.CanAccessUserOwnedResourceAsync(character.UserId.Value, cancellationToken))
        {
            return Result.Failure(CharacterErrors.NotFound(command.Id));
        }

        bool nameExists = await dbContext.Characters
            .AnyAsync(c =>
                c.Id != command.Id &&
                c.UserId == character.UserId &&
                c.Name == name,
                cancellationToken);

        if (nameExists)
        {
            return Result.Failure(CharacterErrors.NameNotUnique);
        }

        character.Name = name;
        character.UpdatedAt = dateTimeProvider.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
