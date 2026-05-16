using Application.Abstractions.Authorization;
using Application.Abstractions.Data;
using Domain.Auth.Users;
using Domain.Game.Characters;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.Game.Characters.CharacterCodes.GetByCharacterId;

internal sealed class GetCodesByCharacterIdQueryHandler(
    IGameDbContext dbContext,
    IUserAccessService userAccessService)
    : IQueryHandler<GetCodesByCharacterIdQuery, IReadOnlyList<CharacterCodeResponse>>
{
    public async Task<Result<IReadOnlyList<CharacterCodeResponse>>> Handle(
        GetCodesByCharacterIdQuery query,
        CancellationToken cancellationToken)
    {
        Character? character = await dbContext.Characters
            .AsNoTracking()
            .Where(c => c.Id == query.CharacterId)
            .SingleOrDefaultAsync(cancellationToken);

        if (character is null)
        {
            return Result.Failure<IReadOnlyList<CharacterCodeResponse>>(CharacterErrors.NotFound(query.CharacterId));
        }

        if (!await userAccessService.CanAccessUserOwnedResourceAsync(character.UserId.Value, cancellationToken))
        {
            return Result.Failure<IReadOnlyList<CharacterCodeResponse>>(UserErrors.Unauthorized());
        }

        IReadOnlyList<CharacterCodeResponse> response = await dbContext.CharacterCodes
            .AsNoTracking()
            .Where(c => c.CharacterId == character.Id)
            .Select(c => new CharacterCodeResponse
            {
                Id = c.Id,
                Name = c.Name,
                SourceCode = c.SourceCode
            })
            .ToListAsync(cancellationToken);

        return Result.Success(response);
    }
}
