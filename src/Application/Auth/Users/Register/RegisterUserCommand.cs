using Application.Abstractions.Messaging;

namespace Application.Auth.Users.Register;

public sealed record RegisterUserCommand(string Email, string FirstName, string LastName, string Password)
    : ICommand<Guid>;
