using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.GameRuntime.GameSessions;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IGameRuntimeDbContext
{
    DbSet<GameSession> GameSessions { get; }

    DbSet<GameActionLogEntry> GameActionLogEntries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
