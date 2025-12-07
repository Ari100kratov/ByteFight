using SharedKernel;

namespace Domain.GameRuntime.GameSessions;

public static class GameSessionErrors
{
    public static Error NotFound(Guid gameSessionId) => Error.NotFound(
        "GameSessions.NotFound",
        $"Игровая сессия с Id = '{gameSessionId}' не найдена");
}
