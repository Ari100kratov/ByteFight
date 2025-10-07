using Application.Abstractions.Messaging;
using Application.Game.Arenas.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Arenas;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("arenas/{id:guid}", async (
            Guid id,
            IQueryHandler<GetArenaByIdQuery, ArenaResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetArenaByIdQuery(id);

            Result<ArenaResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Arenas)
        .RequireAuthorization();
    }
}
