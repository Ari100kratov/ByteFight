using Domain.Auth.RefreshTokens;
using Domain.Auth.Users;
using Domain.Todos;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IAuthDbContext
{
    DbSet<User> Users { get; }

    DbSet<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
