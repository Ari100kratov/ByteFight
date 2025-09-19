using Application.Abstractions.Messaging;

namespace Application.Auth.Users.GetById;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;
