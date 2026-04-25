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
            .Include(x => x.Spec)
                .ThenInclude(x => x.Class)
            .Where(c => c.Id == query.Id && c.UserId == new UserId(userContext.UserId))
            .Select(c => new CharacterResponse(c.Id, c.Name,
                new SpecResponse(
                    c.Spec.Id,
                    c.Spec.Name,
                    c.Spec.Class.Name,
                    c.Spec.Type,
                    c.Spec.Description,
                    c.Spec.Stats.Select(x => x.ToDto()).ToArray(),
                    c.Spec.ActionAssets.Select(x => x.ToDto()).ToArray())))
            .SingleOrDefaultAsync(cancellationToken);

        if (character is null)
        {
            return Result.Failure<CharacterResponse>(CharacterErrors.NotFound(query.Id));
        }

        return Result.Success(character);
    }
}
