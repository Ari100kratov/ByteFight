using Domain.Auth.Users;
using SharedKernel;

namespace Domain.Auth.RefreshTokens;

public sealed class RefreshToken : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }

    public User User { get; set; }
}
