
using SharedKernel.Messaging;

namespace Application.Auth.Users.RefreshTokens;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<RefreshTokenResponse>;
