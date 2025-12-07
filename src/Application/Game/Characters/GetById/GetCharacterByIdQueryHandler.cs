using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain;
using Domain.Game.Characters;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Characters.GetById;

internal sealed class GetCharacterByIdQueryHandler(IGameDbContext dbContext, IUserContext userContext)
    : IQueryHandler<GetCharacterByIdQuery, CharacterResponse>
{
    public async Task<Result<CharacterResponse>> Handle(GetCharacterByIdQuery query, CancellationToken cancellationToken)
    {
        CharacterResponse? character = await dbContext.Characters
            .AsNoTracking()
            .Where(c => c.Id == query.Id && c.UserId == new UserId(userContext.UserId))
            .Select(c => new CharacterResponse(c.Id, c.Name, c.ClassId))
            .SingleOrDefaultAsync(cancellationToken);

        if (character is null)
        {
            return Result.Failure<CharacterResponse>(CharacterErrors.NotFound(query.Id));
        }

        return Result.Success(character);
    }
}
