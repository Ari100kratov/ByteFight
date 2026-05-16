using Application.Abstractions.Authentication;
using Application.Abstractions.Data;

using Domain.Auth.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.Auth.Users.GetCurrent;

internal sealed class GetCurrentUserQueryHandler(IAuthDbContext context, IUserContext userContext)
    : IQueryHandler<GetCurrentUserQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        UserResponse? user = await context.Users
            .AsNoTracking()
            .Where(u => u.Id == userContext.UserId)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Roles = u.UserRoles
                    .Select(ur => ur.Role.Name)
                    .ToArray()
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound(userContext.UserId));
        }

        return user;
    }
}
