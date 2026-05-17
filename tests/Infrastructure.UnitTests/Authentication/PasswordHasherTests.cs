using Infrastructure.Authentication;
using Shouldly;
using Xunit;

namespace Infrastructure.UnitTests.Authentication;

public sealed class PasswordHasherTests
{
    [Fact]
    public void Hash_ShouldCreateDifferentSaltForSamePassword()
    {
        var hasher = new PasswordHasher();

        string firstHash = hasher.Hash("correct horse battery staple");
        string secondHash = hasher.Hash("correct horse battery staple");

        firstHash.ShouldNotBe(secondHash);
        firstHash.Split('-').Length.ShouldBe(2);
    }

    [Fact]
    public void Verify_ShouldAcceptOnlyMatchingPassword()
    {
        var hasher = new PasswordHasher();
        string hash = hasher.Hash("correct horse battery staple");

        hasher.Verify("correct horse battery staple", hash).ShouldBeTrue();
        hasher.Verify("wrong password", hash).ShouldBeFalse();
    }
}
