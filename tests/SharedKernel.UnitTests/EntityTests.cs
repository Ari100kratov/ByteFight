using Shouldly;
using Xunit;

namespace SharedKernel.UnitTests;

public sealed class EntityTests
{
    [Fact]
    public void Raise_ShouldStoreDomainEventsUntilCleared()
    {
        var entity = new TestEntity();
        var domainEvent = new TestDomainEvent(Guid.CreateVersion7());

        entity.Raise(domainEvent);

        entity.DomainEvents.ShouldBe([domainEvent]);

        entity.ClearDomainEvents();

        entity.DomainEvents.ShouldBeEmpty();
    }

    private sealed class TestEntity : Entity;

    private sealed record TestDomainEvent(Guid Id) : IDomainEvent;
}
