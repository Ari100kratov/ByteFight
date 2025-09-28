using System.Security.Cryptography;
using Application.Abstractions.Authentication;
using Domain.Auth.RefreshTokens;
using Domain.Auth.Users;
using SharedKernel;

namespace Infrastructure.Authentication;

internal sealed class RefreshTokenService(IDateTimeProvider dateTimeProvider) : IRefreshTokenService
{
    public RefreshToken Generate(User user, int daysValid = 7)
    {
        string token = GenerateSecureToken();
        DateTime expiresAt = dateTimeProvider.UtcNow.AddDays(daysValid);

        return new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Token = token,
            ExpiresAt = expiresAt,
            IsRevoked = false,
            User = user
        };
    }

    public bool IsValid(RefreshToken token)
    {
        return !token.IsRevoked && token.ExpiresAt > dateTimeProvider.UtcNow;
    }

    public string GenerateSecureToken()
    {
        byte[] bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}
