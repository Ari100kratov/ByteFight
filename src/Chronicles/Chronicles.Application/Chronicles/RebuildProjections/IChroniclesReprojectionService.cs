namespace Chronicles.Application.Chronicles.RebuildProjections;

/// <summary>
/// Пересобирает read-model хроник из уже импортированных inbox-сообщений.
/// </summary>
public interface IChroniclesReprojectionService
{
    Task RebuildAsync(CancellationToken cancellationToken = default);
}
