namespace SharedKernel;

/// <summary>
/// Абстракция текущего времени для доменной логики, сервисов и тестов.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Текущее время в UTC.
    /// </summary>
    DateTime UtcNow { get; }
}
