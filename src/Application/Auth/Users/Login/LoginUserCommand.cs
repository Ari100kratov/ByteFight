using Application.Abstractions.Messaging;

namespace Application.Auth.Users.Login;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<LoginUserResponse>;
