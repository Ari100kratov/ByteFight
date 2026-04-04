using Domain.Auth.RefreshTokens;
using Domain.Auth.Roles;
using Domain.Auth.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IAuthDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RolePermission> RolePermissions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
