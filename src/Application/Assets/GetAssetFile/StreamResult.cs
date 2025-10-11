namespace Application.Assets.GetAssetFile;

public sealed class StreamResult
{
    /// <summary>
    /// Поток с содержимым файла.
    /// </summary>
    public Stream Stream { get; }

    /// <summary>
    /// Имя файла (опционально, для Content-Disposition).
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// MIME-тип контента.
    /// </summary>
    public string ContentType { get; }

    public StreamResult(Stream stream, string contentType, string fileName)
    {
        Stream = stream ?? throw new ArgumentNullException(nameof(stream));
        ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType;
        FileName = fileName ?? "file";
    }
}
