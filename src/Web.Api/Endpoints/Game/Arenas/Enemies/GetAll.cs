using Application.Abstractions.Messaging;
using Application.Game.Arenas.Enemies.Get;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Arenas.Enemies;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("arenas/{arenaId:guid}/enemies", async (
            Guid arenaId,
            IQueryHandler<GetArenaEnemiesQuery, IReadOnlyList<ArenaEnemyResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<ArenaEnemyResponse>> result =
                await handler.Handle(new GetArenaEnemiesQuery(arenaId), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.ArenaEnemies)
        .RequireAuthorization();
    }
}
