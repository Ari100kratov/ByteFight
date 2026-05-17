using Domain.ValueObjects;
using SharedKernel;
using Shouldly;
using Xunit;

namespace Domain.UnitTests.ValueObjects;

public sealed class StatSnapshotTests
{
    [Fact]
    public void Constructor_ShouldRejectInvalidSnapshot()
    {
        Should.Throw<DomainException>(() => new StatSnapshot(-1, 10)).Code.ShouldBe("STAT_NEGATIVE");
        Should.Throw<DomainException>(() => new StatSnapshot(1, 0)).Code.ShouldBe("STAT_INVALID_MAX");
        Should.Throw<DomainException>(() => new StatSnapshot(11, 10)).Code.ShouldBe("STAT_OVERFLOW");
    }

    [Fact]
    public void Constructor_ShouldStoreValidValues()
    {
        var snapshot = new StatSnapshot(7, 10);

        snapshot.Current.ShouldBe(7);
        snapshot.Max.ShouldBe(10);
    }
}
