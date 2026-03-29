using Application.Abstractions.Authentication;
using Application.Abstractions.GameRuntime;
using Domain.Game.GameModes;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.GameRuntime.GameSessions;

internal sealed class Start : IEndpoint
{
    public sealed record Request(Guid ArenaId, string Mode, Guid CharacterId, string Code);

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

            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return CustomResults.Problem(
                    Result.Failure(StartErrors.CodeIsRequired()));
            }


            var init = new GameInitModel(
                userContext.UserId,
                request.ArenaId,
                parsedMode,
                request.CharacterId,
                request.Code ?? string.Empty
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

    public static Error CodeIsRequired() =>
        Error.Validation(
            "GameSession.CodeIsRequired",
            "Пользовательский код обязателен для запуска боя.");
}
