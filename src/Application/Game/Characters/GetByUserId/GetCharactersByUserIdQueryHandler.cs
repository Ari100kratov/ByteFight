using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain;
using Domain.Auth.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Characters.GetByUserId;

public sealed class GetCharactersByUserIdQueryHandler(IGameDbContext dbContext, IUserContext userContext)
    : IQueryHandler<GetCharactersByUserIdQuery, IReadOnlyList<CharacterResponse>>
{
    public async Task<Result<IReadOnlyList<CharacterResponse>>> Handle(GetCharactersByUserIdQuery query, CancellationToken cancellationToken)
    {
        if (userContext.UserId != query.UserId)
        {
            return Result.Failure<IReadOnlyList<CharacterResponse>>(UserErrors.Unauthorized());
        }

        IReadOnlyList<CharacterResponse> characters = await dbContext.Characters
            .AsNoTracking()
            .Where(c => c.UserId == new UserId(query.UserId))
            .Select(c => new CharacterResponse(c.Id, c.Name, c.ClassId))
            .ToListAsync(cancellationToken);

        return Result.Success(characters);
    }
}
