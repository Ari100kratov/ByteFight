using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Auth.RefreshTokens;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Auth.Users.RefreshTokens;

internal sealed class RefreshTokenCommandHandler(
    IAuthDbContext context,
    ITokenProvider tokenProvider,
    IRefreshTokenService refreshTokenService)
    : ICommandHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        RefreshToken? token = await context.RefreshTokens
            .Include(rt => rt.User)
            .SingleOrDefaultAsync(rt => rt.Token == command.RefreshToken, cancellationToken);

        if (token is null || !refreshTokenService.IsValid(token))
        {
            return Result.Failure<RefreshTokenResponse>(RefreshTokenErrors.Invalid());
        }

        token.IsRevoked = true;

        // Сгенерировать новый токен
        RefreshToken newRefreshToken = refreshTokenService.Generate(token.User);
        context.RefreshTokens.Add(newRefreshToken);

        // Новый access-токен
        string accessToken = tokenProvider.Create(token.User);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(new RefreshTokenResponse(accessToken, newRefreshToken.Token));
    }
}
