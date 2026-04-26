using Application.Abstractions.Messaging;

namespace Application.Auth.Users.UpdateProfile;

public sealed record UpdateUserProfileCommand(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName) : ICommand;
