using Application.Abstractions.Authorization;
using Application.Abstractions.Data;
using Domain.Game.Characters;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.Game.Characters.GetById;

internal sealed class GetCharacterByIdQueryHandler(
    IGameDbContext dbContext,
    IUserAccessService userAccessService)
    : IQueryHandler<GetCharacterByIdQuery, CharacterResponse>
{
    public async Task<Result<CharacterResponse>> Handle(GetCharacterByIdQuery query, CancellationToken cancellationToken)
    {
        Character? character = await dbContext.Characters
            .AsNoTracking()
            .Include(x => x.Spec)
            .SingleOrDefaultAsync(c => c.Id == query.Id, cancellationToken);

        if (character is null)
        {
            return Result.Failure<CharacterResponse>(CharacterErrors.NotFound(query.Id));
        }

        if (!await userAccessService.CanAccessUserOwnedResourceAsync(character.UserId.Value, cancellationToken))
        {
            return Result.Failure<CharacterResponse>(CharacterErrors.NotFound(query.Id));
        }

        return Result.Success(new CharacterResponse(
            character.Id,
            character.Name,
            character.Spec.ClassId,
            character.SpecId));
    }
}
