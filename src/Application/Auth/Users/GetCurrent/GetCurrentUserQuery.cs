using Application.Abstractions.Messaging;

namespace Application.Auth.Users.GetCurrent;

public sealed record GetCurrentUserQuery : IQuery<UserResponse>;
