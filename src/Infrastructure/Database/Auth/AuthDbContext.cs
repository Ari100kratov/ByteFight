using Application.Abstractions.Data;
using Domain.Auth.Users;
using Domain.Todos;
using Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Database.Auth;

public sealed class AuthDbContext(
    DbContextOptions<AuthDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher)
    : DbContext(options), IAuthDbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TODO: плохой вариант, в будущем контексты бд вынесу в отдельные проекты
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AuthDbContext).Assembly, t => t.Namespace != null && t.Namespace.Contains("Infrastructure.Database.Auth.Configurations"));

        modelBuilder.HasDefaultSchema(Schemas.Auth);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // When should you publish domain events?
        //
        // 1. BEFORE calling SaveChangesAsync
        //     - domain events are part of the same transaction
        //     - immediate consistency
        // 2. AFTER calling SaveChangesAsync
        //     - domain events are a separate transaction
        //     - eventual consistency
        //     - handlers can fail

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
