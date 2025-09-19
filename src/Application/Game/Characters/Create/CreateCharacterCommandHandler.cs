using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain;
using Domain.Game.Characters;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Characters.Create;

public sealed class CreateCharacterCommandHandler : ICommandHandler<CreateCharacterCommand, Guid>
{
    private readonly IGameDbContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateCharacterCommandHandler(IGameDbContext dbContext, IUserContext userContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<Guid>> Handle(CreateCharacterCommand command, CancellationToken cancellationToken)
    {
        bool exists = await _dbContext.Characters.AnyAsync(c => c.Name == command.Name, cancellationToken);
        if (exists)
        {
            return Result.Failure<Guid>(CharacterErrors.NameNotUnique);
        }

        var character = new Character
        {
            Id = Guid.CreateVersion7(),
            Name = command.Name,
            CreatedAt = _dateTimeProvider.UtcNow,
            UserId = new UserId(_userContext.UserId)
        };

        _dbContext.Characters.Add(character);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(character.Id);
    }
}
