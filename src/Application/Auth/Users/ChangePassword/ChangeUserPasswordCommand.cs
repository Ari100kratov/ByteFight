using Application.Abstractions.Messaging;

namespace Application.Auth.Users.ChangePassword;

public sealed record ChangeUserPasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword) : ICommand;
