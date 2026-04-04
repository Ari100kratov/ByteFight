using Application.Abstractions.Authorization;
using Domain.Auth.Roles;
using Infrastructure.Database.Auth;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Seed;

public sealed class AuthorizationDataSeeder(AuthDbContext dbContext)
{
    public async Task Seed(CancellationToken cancellationToken = default)
    {
        Role adminRole = await EnsureRole(Roles.Admin, cancellationToken);
        Role userRole = await EnsureRole(Roles.User, cancellationToken);

        EnsurePermissions(adminRole, Permissions.All);
        EnsurePermissions(userRole, [Permissions.Users.Access]);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Role> EnsureRole(string roleName, CancellationToken cancellationToken)
    {
        Role? role = await dbContext.Roles
            .Include(r => r.RolePermissions)
            .SingleOrDefaultAsync(r => r.Name == roleName, cancellationToken);

        if (role is not null)
        {
            return role;
        }

        role = new Role
        {
            Id = Guid.CreateVersion7(),
            Name = roleName
        };

        dbContext.Roles.Add(role);

        return role;
    }

    private static void EnsurePermissions(
        Role role,
        IEnumerable<string> permissions)
    {
        var existing = role.RolePermissions
            .Select(x => x.Permission)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (string permission in permissions)
        {
            if (existing.Contains(permission))
            {
                continue;
            }

            role.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id,
                Permission = permission
            });
        }
    }
}
