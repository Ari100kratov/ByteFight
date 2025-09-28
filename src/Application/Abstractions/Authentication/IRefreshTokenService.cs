using Domain.Auth.RefreshTokens;
using Domain.Auth.Users;

namespace Application.Abstractions.Authentication;

public interface IRefreshTokenService
{
    RefreshToken Generate(User user, int daysValid = 7);
    bool IsValid(RefreshToken token);
    string GenerateSecureToken();
}
