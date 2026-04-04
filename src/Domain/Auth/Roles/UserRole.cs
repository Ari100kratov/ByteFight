using Domain.Auth.Users;

namespace Domain.Auth.Roles;

public sealed class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public User User { get; set; }
    public Role Role { get; set; }
}
