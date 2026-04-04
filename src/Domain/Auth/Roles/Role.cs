namespace Domain.Auth.Roles;

public sealed class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public List<UserRole> UserRoles { get; set; } = [];
    public List<RolePermission> RolePermissions { get; set; } = [];
}
