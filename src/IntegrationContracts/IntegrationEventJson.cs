using System.Text.Json;

namespace IntegrationContracts;

/// <summary>
/// Общие настройки JSON-сериализации интеграционных событий.
/// </summary>
public static class IntegrationEventJson
{
    /// <summary>
    /// Настройки сериализации, которые должны использовать producer и consumer интеграционных событий.
    /// </summary>
    public static JsonSerializerOptions SerializerOptions { get; } = CreateSerializerOptions();

    private static JsonSerializerOptions CreateSerializerOptions()
    {
        return new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            // Keeps consumers tolerant if metadata is not the first property in stored JSON.
            AllowOutOfOrderMetadataProperties = true
        };
    }
}
