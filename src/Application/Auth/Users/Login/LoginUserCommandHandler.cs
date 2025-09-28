using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Auth.RefreshTokens;
using Domain.Auth.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Auth.Users.Login;

internal sealed class LoginUserCommandHandler(
    IAuthDbContext context,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider,
    IRefreshTokenService refreshTokenService
    ) : ICommandHandler<LoginUserCommand, LoginUserResponse>
{
    public async Task<Result<LoginUserResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await context.Users
            .SingleOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<LoginUserResponse>(UserErrors.NotFoundByEmail);
        }

        bool verified = passwordHasher.Verify(command.Password, user.PasswordHash);

        if (!verified)
        {
            return Result.Failure<LoginUserResponse>(UserErrors.NotFoundByEmail);
        }

        string accessToken = tokenProvider.Create(user);

        RefreshToken refreshToken = refreshTokenService.Generate(user);
        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(new LoginUserResponse(accessToken, refreshToken.Token));
    }
}
