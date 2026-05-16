namespace Application.Abstractions.Authorization;

/// <summary>
/// Проверяет доступ текущего пользователя к пользовательским данным с учетом административной роли.
/// </summary>
public interface IUserAccessService
{
    /// <summary>
    /// Возвращает признак того, что текущий пользователь имеет роль администратора.
    /// </summary>
    Task<bool> IsCurrentUserAdminAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает признак того, что текущий пользователь может работать с ресурсом указанного владельца.
    /// </summary>
    Task<bool> CanAccessUserOwnedResourceAsync(Guid ownerUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает признак того, что текущий пользователь может работать с ресурсом одного из указанных владельцев.
    /// </summary>
    Task<bool> CanAccessAnyUserOwnedResourceAsync(
        IReadOnlyCollection<Guid> ownerUserIds,
        CancellationToken cancellationToken = default);
}
