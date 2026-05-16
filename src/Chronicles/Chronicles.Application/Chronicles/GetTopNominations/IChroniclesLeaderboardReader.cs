using SharedKernel;

namespace Chronicles.Application.Chronicles.GetTopNominations;

/// <summary>
/// Читает лидерборды хроник по номинациям.
/// </summary>
public interface IChroniclesLeaderboardReader
{
    /// <summary>
    /// Возвращает топ записей по всем номинациям.
    /// </summary>
    Task<Result<IReadOnlyList<ChronicleNominationLeaderboardResponse>>> GetTopAsync(int top, CancellationToken cancellationToken);
}
