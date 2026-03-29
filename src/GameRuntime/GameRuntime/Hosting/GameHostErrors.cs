using Application.Abstractions.GameRuntime;
using SharedKernel;

namespace GameRuntime.Hosting;

internal static class GameHostErrors
{
    public static Error NotFound(Guid sessionId) =>
        Error.NotFound(
            "GameHost.NotRunning",
            $"Игровая сессия с Id = {sessionId} не найдена.");

    public static Error AlreadyRunningForUser(Guid userId) =>
        Error.Conflict(
            "GameHost.AlreadyRunningForUser",
            "У вас уже запущена другая игровая сессия. Завершите текущий бой перед запуском нового.");

    public static Error SessionAlreadyRegistered(Guid sessionId) =>
        Error.Conflict(
            "GameHost.SessionAlreadyRegistered",
            $"Игровая сессия с Id = {sessionId} уже зарегистрирована в хосте.");

    public static Error UserCodeCompilationFailed(string description) =>
        Error.Validation(
            "GameHost.UserCodeCompilationFailed",
            description);

    public static Error StartFailure(GameInitModel initModel) =>
        Error.Failure(
            "GameHost.StartFailed",
            $"Не удалось начать игровую сессию. UserId {initModel.UserId}, ArenaId {initModel.ArenaId}, CharacterId {initModel.CharacterId}, Mode {initModel.Mode}");
}
