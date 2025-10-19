using Application.Abstractions.Messaging;
using Application.Game.GameModes.Get;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.GameModes;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("game/modes", async (
            IQueryHandler<GetGameModesQuery, IReadOnlyList<GameModeResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetGameModesQuery();

            Result<IReadOnlyList<GameModeResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.GameModes)
        .RequireAuthorization();
    }
}
