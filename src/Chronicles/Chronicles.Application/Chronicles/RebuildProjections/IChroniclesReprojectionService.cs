namespace Chronicles.Application.Chronicles.RebuildProjections;

/// <summary>
/// Пересобирает read-model хроник из уже импортированных inbox-сообщений.
/// </summary>
public interface IChroniclesReprojectionService
{
    /// <summary>
    /// Очищает read-model хроник и пересобирает её из уже импортированных inbox-сообщений.
    /// </summary>
    Task RebuildAsync(CancellationToken cancellationToken = default);
}
