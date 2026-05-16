using Infrastructure.Database.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Authorization;

internal sealed class PermissionProvider(
    AuthDbContext dbContext,
    IMemoryCache cache)
{
    private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(10);

    public Task<HashSet<string>> GetForUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        string cacheKey = GetCacheKey(userId);

        return cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheLifetime;

            string[] permissions = await dbContext.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission))
                .Distinct()
                .ToArrayAsync(cancellationToken);

            return permissions.ToHashSet(StringComparer.OrdinalIgnoreCase);
        })!;
    }

    public Task<HashSet<string>> GetRolesForUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        string cacheKey = GetRolesCacheKey(userId);

        return cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheLifetime;

            string[] roles = await dbContext.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.Name)
                .Distinct()
                .ToArrayAsync(cancellationToken);

            return roles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        })!;
    }

    public async Task<bool> IsInRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        HashSet<string> roles = await GetRolesForUserIdAsync(userId, cancellationToken);

        return roles.Contains(roleName);
    }

    public void Invalidate(Guid userId)
    {
        cache.Remove(GetCacheKey(userId));
        cache.Remove(GetRolesCacheKey(userId));
    }

    private static string GetCacheKey(Guid userId) => $"auth:permissions:{userId}";

    private static string GetRolesCacheKey(Guid userId) => $"auth:roles:{userId}";
}
