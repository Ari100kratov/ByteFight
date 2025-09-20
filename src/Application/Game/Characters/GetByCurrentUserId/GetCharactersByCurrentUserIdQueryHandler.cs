using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain;
using Domain.Auth.Users;
using Domain.Game.Characters;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Characters.GetByCurrentUserId;

public sealed class GetCharactersByCurrentUserIdQueryHandler(IGameDbContext dbContext, IUserContext userContext)
    : IQueryHandler<GetCharactersByCurrentUserIdQuery, IReadOnlyList<CharacterResponse>>
{
    public async Task<Result<IReadOnlyList<CharacterResponse>>> Handle(GetCharactersByCurrentUserIdQuery query, CancellationToken cancellationToken)
    {
        IReadOnlyList<CharacterResponse> characters = await dbContext.Characters
            .AsNoTracking()
            .Where(c => c.UserId == new UserId(userContext.UserId))
            .Select(c => new CharacterResponse
            {
                Id = c.Id,
                Name = c.Name,
                UserId = c.UserId.Value
            })
            .ToListAsync(cancellationToken);

        return Result.Success(characters);
    }
}
