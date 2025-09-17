using Domain.Game.Characters;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IGameDbContext
{
    DbSet<Character> Characters { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
