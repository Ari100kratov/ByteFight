using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Game.Characters;
using Domain.Auth.Users;
using Domain.Game.Characters;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Characters.GetById;

internal sealed class GetCharacterByIdQueryHandler : IQueryHandler<GetCharacterByIdQuery, CharacterResponse>
{
    private readonly IGameDbContext _dbContext;
    private readonly IUserContext _userContext;

    public GetCharacterByIdQueryHandler(IGameDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<Result<CharacterResponse>> Handle(GetCharacterByIdQuery query, CancellationToken cancellationToken)
    {
        CharacterResponse? character = await _dbContext.Characters
            .AsNoTracking()
            .Where(c => c.Id == query.Id)
            .Select(c => new CharacterResponse
            {
                Id = c.Id,
                Name = c.Name,
                UserId = c.UserId.Value
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (character is null)
        {
            return Result.Failure<CharacterResponse>(CharacterErrors.NotFound(query.Id));
        }

        if (_userContext.UserId != character.UserId)
        {
            return Result.Failure<CharacterResponse>(UserErrors.Unauthorized());
        }

        return Result.Success(character);
    }
}
