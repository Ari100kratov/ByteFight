using Chronicles.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chronicles.Application.Abstractions.Data;

/// <summary>
/// Контракт доступа к данным подсистемы хроник.
/// </summary>
public interface IChroniclesDbContext
{
    /// <summary>
    /// Справочник номинаций хроник.
    /// </summary>
    DbSet<ChronicleNomination> ChronicleNominations { get; }

    /// <summary>
    /// Исторические записи по номинациям.
    /// </summary>
    DbSet<ChronicleRecord> ChronicleRecords { get; }

    /// <summary>
    /// Текущие результаты персонажей по номинациям.
    /// </summary>
    DbSet<CharacterNominationScore> CharacterNominationScores { get; }

    /// <summary>
    /// Агрегированная статистика персонажей.
    /// </summary>
    DbSet<CharacterChronicleStats> CharacterChronicleStats { get; }

    /// <summary>
    /// Входящие интеграционные сообщения.
    /// </summary>
    DbSet<InboxMessage> InboxMessages { get; }

    /// <summary>
    /// Сохраняет изменения.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
