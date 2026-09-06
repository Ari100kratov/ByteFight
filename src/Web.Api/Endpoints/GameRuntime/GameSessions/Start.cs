using Application.Abstractions.Authentication;
using Application.Abstractions.GameRuntime;
using Domain.Game.GameModes;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.GameRuntime.GameSessions;

internal sealed class Start : IEndpoint
{
    /// <summary>
    /// Запуск боя. Код опционален: без кода бой идёт в ручном режиме
    /// (игрок управляет персонажем сам), с кодом — в скрытом скриптовом.
    /// </summary>
    public sealed record Request(Guid ArenaId, string Mode, Guid CharacterId, string? Code = null);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("game/start", async (
            Request request,
            IGameHost gameHost,
            IUserContext userContext,
            CancellationToken ct) =>
        {
            // TODO: передавать enum в API
            if (!Enum.TryParse(request.Mode, true, out GameModeType parsedMode))
            {
                return CustomResults.Problem(
                    Result.Failure(StartErrors.InvalidMode(request.Mode)));
            }

            var init = new GameInitModel(
                userContext.UserId,
                request.ArenaId,
                parsedMode,
                request.CharacterId,
                string.IsNullOrWhiteSpace(request.Code) ? null : request.Code
            );

            Result<Guid> result = await gameHost.StartGame(init, ct);
            return result.ToCreated(id => $"/game/{id}");
        })
        .WithTags(Tags.Game)
        .RequireAuthorization();
    }
}

internal static class StartErrors
{
    public static Error InvalidMode(string mode) =>
        Error.Validation(
            "GameSession.InvalidMode",
            $"Неизвестный режим игры: {mode}");
}
