using Microsoft.EntityFrameworkCore;

namespace Chronicles.Infrastructure.Source.GameRuntime;

internal sealed class SourceGameRuntimeReadDbContext(DbContextOptions<SourceGameRuntimeReadDbContext> options)
    : DbContext(options)
{
    public DbSet<OutboxMessageReadModel> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SourceGameRuntimeReadDbContext).Assembly,
            type => type.Namespace is not null && type.Namespace.Contains("Chronicles.Infrastructure.Source.GameRuntime.Configurations", StringComparison.Ordinal));
    }
}
