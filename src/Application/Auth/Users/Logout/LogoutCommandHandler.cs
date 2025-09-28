using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Auth.RefreshTokens;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Auth.Users.Logout;

internal sealed class LogoutCommandHandler(
    IAuthDbContext context,
    IUserContext userContext
) : ICommandHandler<LogoutCommand>
{
    public async Task<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        List<RefreshToken> tokens = await context.RefreshTokens
            .Where(rt => rt.UserId == userContext.UserId)
            .ToListAsync(cancellationToken);

        foreach (RefreshToken token in tokens)
        {
            token.IsRevoked = true;
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
