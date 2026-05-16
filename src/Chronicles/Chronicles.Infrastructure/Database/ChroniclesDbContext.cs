using Chronicles.Application.Abstractions.Data;
using Chronicles.Domain;
using Chronicles.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Chronicles.Infrastructure.Database;

/// <summary>
/// EF Core контекст read-model и inbox подсистемы хроник.
/// </summary>
public sealed class ChroniclesDbContext(DbContextOptions<ChroniclesDbContext> options)
    : DbContext(options), IChroniclesDbContext
{
    /// <summary>
    /// Справочник номинаций хроник.
    /// </summary>
    public DbSet<ChronicleNomination> ChronicleNominations { get; set; }

    /// <summary>
    /// Исторические записи по номинациям.
    /// </summary>
    public DbSet<ChronicleRecord> ChronicleRecords { get; set; }

    /// <summary>
    /// Текущие результаты персонажей по номинациям.
    /// </summary>
    public DbSet<CharacterNominationScore> CharacterNominationScores { get; set; }

    /// <summary>
    /// Агрегированная статистика персонажей.
    /// </summary>
    public DbSet<CharacterChronicleStats> CharacterChronicleStats { get; set; }

    /// <summary>
    /// Входящие интеграционные сообщения.
    /// </summary>
    public DbSet<InboxMessage> InboxMessages { get; set; }

    /// <summary>
    /// Курсоры импорта внешних outbox-сообщений.
    /// </summary>
    internal DbSet<OutboxImportCursor> ImportCursors { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ChroniclesDbContext).Assembly,
            type => type.Namespace is not null && type.Namespace.Contains("Chronicles.Infrastructure.Database.Configurations", StringComparison.Ordinal));

        modelBuilder.HasDefaultSchema(Schemas.Chronicles);
    }
}
