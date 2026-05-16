
using SharedKernel.Messaging;

namespace Application.Auth.Users.Login;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<LoginUserResponse>;
