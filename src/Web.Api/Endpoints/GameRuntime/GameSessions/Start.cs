using Application.Abstractions.Authentication;
using Application.Abstractions.GameRuntime;
using Domain.Game.GameModes;
using SharedKernel;
using Web.Api.Extensions;

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
                return Results.BadRequest($"Invalid mode: {parsedMode}");
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
