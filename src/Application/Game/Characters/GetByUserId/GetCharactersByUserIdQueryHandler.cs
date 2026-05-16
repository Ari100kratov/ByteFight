using Application.Abstractions.Authorization;
using Application.Abstractions.Data;
using Domain;
using Domain.Auth.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.Game.Characters.GetByUserId;

public sealed class GetCharactersByUserIdQueryHandler(
    IGameDbContext dbContext,
    IUserAccessService userAccessService)
    : IQueryHandler<GetCharactersByUserIdQuery, IReadOnlyList<CharacterResponse>>
{
    public async Task<Result<IReadOnlyList<CharacterResponse>>> Handle(
        GetCharactersByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        if (!await userAccessService.CanAccessUserOwnedResourceAsync(query.UserId, cancellationToken))
        {
            return Result.Failure<IReadOnlyList<CharacterResponse>>(UserErrors.Unauthorized());
        }

        IReadOnlyList<CharacterResponse> characters = await dbContext.Characters
            .AsNoTracking()
            .Include(x => x.Spec)
                .ThenInclude(x => x.Class)
            .Where(c => c.UserId == new UserId(query.UserId))
            .OrderBy(a => a.CreatedAt)
            .Select(c => new CharacterResponse(c.Id, c.Name, c.Spec.Class.Name, c.Spec.Name))
            .ToListAsync(cancellationToken);

        return Result.Success(characters);
    }
}
