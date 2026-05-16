using Application.Abstractions.Authentication;
using Application.Abstractions.Authorization;

namespace Infrastructure.Authorization;

internal sealed class UserAccessService(
    IUserContext userContext,
    PermissionProvider permissionProvider)
    : IUserAccessService
{
    public Task<bool> IsCurrentUserAdminAsync(CancellationToken cancellationToken = default) =>
        permissionProvider.IsInRoleAsync(userContext.UserId, Roles.Admin, cancellationToken);

    public async Task<bool> CanAccessUserOwnedResourceAsync(
        Guid ownerUserId,
        CancellationToken cancellationToken = default)
    {
        if (ownerUserId == userContext.UserId)
        {
            return true;
        }

        return await IsCurrentUserAdminAsync(cancellationToken);
    }

    public async Task<bool> CanAccessAnyUserOwnedResourceAsync(
        IReadOnlyCollection<Guid> ownerUserIds,
        CancellationToken cancellationToken = default)
    {
        if (ownerUserIds.Contains(userContext.UserId))
        {
            return true;
        }

        return await IsCurrentUserAdminAsync(cancellationToken);
    }
}
