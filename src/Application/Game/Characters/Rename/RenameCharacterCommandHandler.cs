using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain;
using Domain.Game.Characters;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Characters.Rename;

internal sealed class RenameCharacterCommandHandler(
    IGameDbContext dbContext,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<RenameCharacterCommand>
{
    public async Task<Result> Handle(RenameCharacterCommand command, CancellationToken cancellationToken)
    {
        string name = command.Name.Trim();
        var userId = new UserId(userContext.UserId);

        Character? character = await dbContext.Characters
            .SingleOrDefaultAsync(c => c.Id == command.Id && c.UserId == userId, cancellationToken);

        if (character is null)
        {
            return Result.Failure(CharacterErrors.NotFound(command.Id));
        }

        bool nameExists = await dbContext.Characters
            .AnyAsync(c =>
                c.Id != command.Id &&
                c.UserId == userId &&
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
