using Application.Abstractions.Messaging;

namespace Application.Auth.Users.RefreshTokens;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<RefreshTokenResponse>;
