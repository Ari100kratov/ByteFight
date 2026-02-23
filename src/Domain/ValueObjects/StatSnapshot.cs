using SharedKernel;

namespace Domain.ValueObjects;

/// <summary>
/// Снапшот какой-либо характеристики
/// </summary>
public sealed record StatSnapshot
{
    /// <summary>
    /// Текущее значение
    /// </summary>
    public decimal Current { get; init; }

    /// <summary>
    /// Максимальное/начальное значение
    /// </summary>
    public decimal Max { get; init; }

    private StatSnapshot() { } // EF

    public StatSnapshot(decimal current, decimal max)
    {
        if (current < 0)
        {
            throw new DomainException("STAT_NEGATIVE", "Current value cannot be negative.");
        }

        if (max <= 0)
        {
            throw new DomainException("STAT_INVALID_MAX", "Max value must be greater than zero.");
        }

        if (current > max)
        {
            throw new DomainException("STAT_OVERFLOW", "Current cannot exceed max.");
        }

        Current = current;
        Max = max;
    }
}
