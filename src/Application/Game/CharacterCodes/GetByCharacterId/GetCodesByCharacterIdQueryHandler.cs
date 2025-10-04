using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Auth.Users;
using Domain.Game.Characters;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.CharacterCodes.GetByCharacterId;

internal sealed class GetCodesByCharacterIdQueryHandler(IGameDbContext dbContext, IUserContext userContext)
    : IQueryHandler<GetCodesByCharacterIdQuery, IReadOnlyList<CharacterCodeResponse>>
{
    public async Task<Result<IReadOnlyList<CharacterCodeResponse>>> Handle(GetCodesByCharacterIdQuery query, CancellationToken cancellationToken)
    {
        Character? character = await dbContext.Characters
            .AsNoTracking()
            .Where(c => c.Id == query.CharacterId)
            .SingleOrDefaultAsync(cancellationToken);

        if (character is null)
        {
            return Result.Failure<IReadOnlyList<CharacterCodeResponse>>(CharacterErrors.NotFound(query.CharacterId));
        }

        if (userContext.UserId != character.UserId.Value)
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
