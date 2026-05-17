using Infrastructure.Authorization;
using Shouldly;
using Xunit;

namespace Infrastructure.UnitTests.Authorization;

public sealed class HasPermissionAttributeTests
{
    [Fact]
    public void Constructor_ShouldUsePermissionAsAuthorizationPolicy()
    {
        var attribute = new HasPermissionAttribute("arenas:create");

        attribute.Policy.ShouldBe("arenas:create");
    }
}
