using Application.Abstractions.Data;
using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.GameRuntime.GameSessionParticipants;
using Domain.GameRuntime.GameSessions;
using Domain.Integration;
using Infrastructure.Database.Game;
using Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Database.GameRuntime;

public sealed class GameRuntimeDbContext(
    DbContextOptions<GameRuntimeDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher)
    : DbContext(options), IGameRuntimeDbContext
{
    public DbSet<GameSession> GameSessions { get; set; }

    public DbSet<GameSessionParticipant> GameSessionParticipants { get; set; }

    public DbSet<GameActionLogEntry> GameActionLogEntries { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(GameDbContext).Assembly, t => t.Namespace != null && t.Namespace.Contains("Infrastructure.Database.GameRuntime.Configurations"));

        modelBuilder.HasDefaultSchema(Schemas.GameRuntime);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync();

        return result;
    }

    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                List<IDomainEvent> domainEvents = entity.DomainEvents;

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .ToList();

        await domainEventsDispatcher.DispatchAsync(domainEvents);
    }
}
