using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.GameRuntime.GameSessions;
using Domain.Integration;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IGameRuntimeDbContext
{
    DbSet<GameSession> GameSessions { get; }

    DbSet<GameActionLogEntry> GameActionLogEntries { get; }

    DbSet<OutboxMessage> OutboxMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
