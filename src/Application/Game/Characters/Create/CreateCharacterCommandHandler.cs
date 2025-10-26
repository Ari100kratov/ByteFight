using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain;
using Domain.Game.CharacterClasses;
using Domain.Game.Characters;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Characters.Create;

internal sealed class CreateCharacterCommandHandler(
    IGameDbContext dbContext,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<CreateCharacterCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCharacterCommand command, CancellationToken cancellationToken)
    {
        string name = command.Name.Trim();

        bool exists = await dbContext.Characters.AnyAsync(c => c.Name == name, cancellationToken);
        if (exists)
        {
            return Result.Failure<Guid>(CharacterErrors.NameNotUnique);
        }

        bool characterClassExists = await dbContext.CharacterClasses
            .AnyAsync(x => x.Id == command.ClassId, cancellationToken);

        if (!characterClassExists)
        {
            return Result.Failure<Guid>(CharacterClassErrors.NotFound(command.ClassId));
        }

        var character = new Character
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            ClassId = command.ClassId,
            CreatedAt = dateTimeProvider.UtcNow,
            UserId = new UserId(userContext.UserId)
        };

        dbContext.Characters.Add(character);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(character.Id);
    }
}
