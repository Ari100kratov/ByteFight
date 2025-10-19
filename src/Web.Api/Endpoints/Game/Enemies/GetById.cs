using Application.Abstractions.Messaging;
using Application.Game.Enemies.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Enemies;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("enemies/{id:guid}", async (
            Guid id,
            IQueryHandler<GetEnemyByIdQuery, EnemyResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<EnemyResponse> result = await handler.Handle(new GetEnemyByIdQuery(id), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Enemies)
        .RequireAuthorization();
    }
}
