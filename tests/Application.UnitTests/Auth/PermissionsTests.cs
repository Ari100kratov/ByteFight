using Application.Abstractions.Authorization;
using Shouldly;
using Xunit;

namespace Application.UnitTests.Auth;

public sealed class PermissionsTests
{
    [Fact]
    public void All_ShouldContainEveryDeclaredPermissionOnce()
    {
        string[] declaredPermissions =
        [
            Permissions.Users.Access,
            Permissions.Arenas.Create,
            Permissions.Arenas.Edit,
            Permissions.Arenas.Delete,
            Permissions.ArenaEnemies.Add,
            Permissions.ArenaEnemies.Remove,
            Permissions.Enemies.Create,
            Permissions.Enemies.Edit,
            Permissions.Enemies.Delete,
            Permissions.CharacterClasses.Create,
            Permissions.CharacterClasses.Edit,
            Permissions.CharacterClasses.Delete,
            Permissions.CharacterSpecs.Create,
            Permissions.CharacterSpecs.Edit,
            Permissions.CharacterSpecs.Delete
        ];

        Permissions.All.ShouldBe(declaredPermissions);
        Permissions.All.Distinct(StringComparer.Ordinal).Count().ShouldBe(Permissions.All.Length);
    }
}
