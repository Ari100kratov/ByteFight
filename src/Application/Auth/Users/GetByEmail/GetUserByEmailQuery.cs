using Application.Abstractions.Messaging;

namespace Application.Auth.Users.GetByEmail;

public sealed record GetUserByEmailQuery(string Email) : IQuery<UserResponse>;
