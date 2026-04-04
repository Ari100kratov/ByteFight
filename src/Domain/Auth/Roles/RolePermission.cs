namespace Domain.Auth.Roles;

public sealed class RolePermission
{
    public Guid RoleId { get; set; }
    public string Permission { get; set; }

    public Role Role { get; set; }
}
