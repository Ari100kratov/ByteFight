using Domain.Game.GameModes;
using SharedKernel;

namespace Application.Abstractions.GameRuntime;

public interface IGameHost
{
    Task<Result<Guid>> StartGame(GameInitModel initModel, CancellationToken ct);

    Result CancelGame(Guid sessionId);
}

/// <summary>
/// Модель запуска боя. Пустой или отсутствующий код — ручное управление;
/// код — скрытый скриптовый режим для совместимости.
/// </summary>
public sealed record GameInitModel(
    Guid UserId,
    Guid ArenaId,
    GameModeType Mode,
    Guid CharacterId,
    string? Code = null);
