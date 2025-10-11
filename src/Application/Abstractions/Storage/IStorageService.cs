namespace Application.Abstractions.Storage;

/// <summary>
/// Абстракция для работы с файловым хранилищем.
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Получает поток файла по указанному бакету и ключу.
    /// Используется для чтения содержимого файла.
    /// </summary>
    /// <param name="bucket">Имя бакета (контейнера) в хранилище.</param>
    /// <param name="key">Ключ файла в бакете.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Поток с данными файла.</returns>
    Task<Stream> GetFileStreamAsync(string bucket, string key, CancellationToken cancellationToken);

    /// <summary>
    /// Получает публичный или временный URL для доступа к файлу по ключу.
    /// URL может быть использован для прямого скачивания или отображения файла.
    /// </summary>
    /// <param name="bucket">Имя бакета (контейнера) в хранилище.</param>
    /// <param name="key">Ключ файла в бакете.</param>
    /// <param name="lifetime">Необязательный срок действия временного URL. Если не указан — может возвращаться постоянный URL.</param>
    /// <returns>Строка с URL файла.</returns>
    Task<string> GetFileUrlAsync(string bucket, string key, TimeSpan? lifetime = null);

    /// <summary>
    /// Проверяет, существует ли объект (файл) с указанным ключом в бакете.
    /// </summary>
    /// <param name="bucket">Имя бакета (контейнера) в хранилище.</param>
    /// <param name="key">Ключ файла в бакете.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если объект существует, иначе false.</returns>
    Task<bool> ExistsAsync(string bucket, string key, CancellationToken cancellationToken);
}
