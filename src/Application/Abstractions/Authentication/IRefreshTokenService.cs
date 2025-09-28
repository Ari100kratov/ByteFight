using Domain.Auth.Users;
using Domain.Auth.RefreshTokens;

namespace Application.Abstractions.Authentication;

public interface IRefreshTokenService
{
    RefreshToken Generate(User user, int daysValid = 7);
    bool IsValid(RefreshToken token);
    string GenerateSecureToken();
}
