using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Domain;
using Domain.Game.Characters;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Characters.GetDetails;

internal sealed class GetCharacterDetailsQueryHandler(IGameDbContext dbContext, IUserContext userContext)
    : IQueryHandler<GetCharacterDetailsQuery, CharacterResponse>
{
    public async Task<Result<CharacterResponse>> Handle(GetCharacterDetailsQuery query, CancellationToken cancellationToken)
    {
        CharacterResponse? character = await dbContext.Characters
            .AsNoTracking()
            .Include(x => x.Class)
            .Where(c => c.Id == query.Id && c.UserId == new UserId(userContext.UserId))
            .Select(c => new CharacterResponse(c.Id, c.Name,
                new ClassResponse(
                    c.Class.Id,
                    c.Class.Name,
                    c.Class.Type,
                    c.Class.Description,
                    c.Class.Stats.Select(x => x.ToDto()).ToArray(),
                    c.Class.ActionAssets.Select(x => x.ToDto()).ToArray())))
            .SingleOrDefaultAsync(cancellationToken);

        if (character is null)
        {
            return Result.Failure<CharacterResponse>(CharacterErrors.NotFound(query.Id));
        }

        return Result.Success(character);
    }
}
