using Application.Abstractions.Messaging;
using Application.Game.Arenas.GetByMode;
using Domain.Game.GameModes;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Arenas;

internal sealed class GetByMode : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("arenas", async (
            string mode,
            IQueryHandler<GetArenasByModeQuery, IReadOnlyList<ArenaResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            // TODO: сделать по-человечески парсинг перечисления из query-параметра
            if (!Enum.TryParse(mode, true, out GameModeType parsedMode))
            {
                return Results.BadRequest($"Invalid mode: {mode}");
            }

            var query = new GetArenasByModeQuery(parsedMode);

            Result<IReadOnlyList<ArenaResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Arenas)
        .RequireAuthorization();
    }
}
