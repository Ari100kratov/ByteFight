using Chronicles.Application.Abstractions.Data;

namespace Chronicles.Application.Chronicles.ProcessCompletedGameSessions;

/// <summary>
/// Преобразует сохранённый JSON payload интеграционного события в данные, пригодные для расчёта хроник.
/// </summary>
public interface ICompletedGameSessionPayloadParser
{
    /// <summary>
    /// Разбирает payload завершённой игровой сессии и проверяет поддерживаемую версию схемы.
    /// </summary>
    CompletedGameSessionData Parse(string payload);
}
