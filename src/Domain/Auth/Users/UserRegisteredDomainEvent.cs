using SharedKernel;

namespace Domain.Auth.Users;

public sealed record UserRegisteredDomainEvent(Guid UserId) : IDomainEvent;
