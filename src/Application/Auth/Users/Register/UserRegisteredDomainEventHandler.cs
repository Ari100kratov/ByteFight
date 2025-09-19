using Domain.Auth.Users;
using SharedKernel;

namespace Application.Auth.Users.Register;

internal sealed class UserRegisteredDomainEventHandler : IDomainEventHandler<UserRegisteredDomainEvent>
{
    public Task Handle(UserRegisteredDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // TODO: Send an email verification link, etc.
        return Task.CompletedTask;
    }
}
