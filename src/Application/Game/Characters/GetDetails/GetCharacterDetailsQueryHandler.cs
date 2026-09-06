using Application.Abstractions.Authorization;
using Application.Abstractions.Data;
using Application.Contracts;
using Domain.Game.Characters;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.Game.Characters.GetDetails;

internal sealed class GetCharacterDetailsQueryHandler(
    IGameDbContext dbContext,
    IUserAccessService userAccessService)
    : IQueryHandler<GetCharacterDetailsQuery, CharacterResponse>
{
    public async Task<Result<CharacterResponse>> Handle(GetCharacterDetailsQuery query, CancellationToken cancellationToken)
    {
        Character? character = await dbContext.Characters
            .AsNoTracking()
            .Include(x => x.Spec)
                .ThenInclude(x => x.Class)
            .Include(x => x.Spec)
                .ThenInclude(x => x.Stats)
            .Include(x => x.Spec)
                .ThenInclude(x => x.ActionAssets)
            .Include(x => x.Spec)
                .ThenInclude(x => x.Abilities)
                    .ThenInclude(a => a.Stats)
            .Include(x => x.Spec)
                .ThenInclude(x => x.Abilities)
                    .ThenInclude(a => a.ActionAssets)
            .SingleOrDefaultAsync(c => c.Id == query.Id, cancellationToken);

        if (character is null)
        {
            return Result.Failure<CharacterResponse>(CharacterErrors.NotFound(query.Id));
        }

        if (!await userAccessService.CanAccessUserOwnedResourceAsync(character.UserId.Value, cancellationToken))
        {
            return Result.Failure<CharacterResponse>(CharacterErrors.NotFound(query.Id));
        }

        var response = new CharacterResponse(
            character.Id,
            character.Name,
            character.Level,
            new SpecResponse(
                character.Spec.Id,
                character.Spec.Name,
                character.Spec.Class.Name,
                character.Spec.Type,
                character.Spec.Description,
                [.. character.Spec.Stats.Select(x => x.ToDto())],
                [.. character.Spec.ActionAssets.Select(x => x.ToDto())],
                [.. character.Spec.Abilities.Select(x => x.ToDto())]));

        return Result.Success(response);
    }
}
