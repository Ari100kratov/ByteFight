using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authorization;

/// <summary>
/// Подключает policy-based проверку права доступа к endpoint'у или группе endpoint'ов.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Создаёт атрибут проверки указанного permission.
    /// </summary>
    public HasPermissionAttribute(string permission)
        : base(permission)
    {
    }
}
