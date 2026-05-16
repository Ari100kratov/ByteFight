using Application.Abstractions.Authentication;
using Application.Abstractions.Data;

using Domain;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.Game.Characters.GetByCurrentUserId;

public sealed class GetCharactersByCurrentUserIdQueryHandler(IGameDbContext dbContext, IUserContext userContext)
    : IQueryHandler<GetCharactersByCurrentUserIdQuery, IReadOnlyList<CharacterResponse>>
{
    public async Task<Result<IReadOnlyList<CharacterResponse>>> Handle(GetCharactersByCurrentUserIdQuery query, CancellationToken cancellationToken)
    {
        IReadOnlyList<CharacterResponse> characters = await dbContext.Characters
            .AsNoTracking()
            .Include(x => x.Spec)
                .ThenInclude(x => x.Class)
            .Where(c => c.UserId == new UserId(userContext.UserId))
            .OrderBy(a => a.CreatedAt)
            .Select(c => new CharacterResponse(
                c.Id,
                c.Name,
                c.Spec.Class.Name,
                c.Spec.Name,
                new Uri(c.Spec.PortraitUrl, UriKind.Relative)))
            .ToListAsync(cancellationToken);

        return Result.Success(characters);
    }
}
