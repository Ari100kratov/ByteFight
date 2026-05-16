
using SharedKernel.Messaging;

namespace Application.Auth.Users.GetById;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;
