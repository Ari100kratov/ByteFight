using Infrastructure.Database.Game;
using Infrastructure.Database.GameRuntime;
using Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SharedKernel;

namespace Infrastructure.Database;

/// <summary>
/// Фабрики контекстов для генерации миграций командой dotnet ef.
/// Подставная строка подключения не используется: применяется только модель.
/// </summary>
internal static class DesignConnectionString
{
    // Заглушка для design-time: миграции читают только модель контекста,
    // реальное подключение не выполняется.
    public static string Value { get; } = new Npgsql.NpgsqlConnectionStringBuilder
    {
        Host = "localhost",
        Database = "bytefight-design",
        Username = "design",
#pragma warning disable S2068 // Заглушка для генерации миграций, не настоящий пароль.
        Password = "design-only-placeholder"
#pragma warning restore S2068
    }.ToString();
}

public sealed class GameDbContextFactory : IDesignTimeDbContextFactory<GameDbContext>
{
    public GameDbContext CreateDbContext(string[] args)
    {
        DbContextOptions<GameDbContext> options = new DbContextOptionsBuilder<GameDbContext>()
            .UseNpgsql(DesignConnectionString.Value)
            .Options;

        return new GameDbContext(options, new NoOpDomainEventsDispatcher());
    }
}

public sealed class GameRuntimeDbContextFactory : IDesignTimeDbContextFactory<GameRuntimeDbContext>
{
    public GameRuntimeDbContext CreateDbContext(string[] args)
    {
        DbContextOptions<GameRuntimeDbContext> options = new DbContextOptionsBuilder<GameRuntimeDbContext>()
            .UseNpgsql(DesignConnectionString.Value)
            .Options;

        return new GameRuntimeDbContext(options, new NoOpDomainEventsDispatcher());
    }
}

internal sealed class NoOpDomainEventsDispatcher : IDomainEventsDispatcher
{
    public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
