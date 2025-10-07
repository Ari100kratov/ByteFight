using Domain.Game.Arenas;
using Domain.Game.CharacterCodes;
using Domain.Game.Characters;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IGameDbContext
{
    DbSet<Character> Characters { get; }

    DbSet<CharacterCode> CharacterCodes { get; }

    DbSet<Arena> Arenas { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
