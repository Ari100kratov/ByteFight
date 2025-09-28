using SharedKernel;

namespace Domain.Auth.RefreshTokens;

public static class RefreshTokenErrors
{
    public static Error Invalid() => Error.Failure(
        "Users.InvalidRefreshToken",
        "Токен недействителен");
}
