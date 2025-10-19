using Application.Abstractions.Messaging;
using Application.Game.Arenas.Enemies.Remove;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Arenas.Enemies;

internal sealed class Remove : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("arenas/{arenaId:guid}/enemies/{enemyId:guid}", async (
            Guid arenaId,
            Guid enemyId,
            ICommandHandler<RemoveEnemyFromArenaCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RemoveEnemyFromArenaCommand(arenaId, enemyId);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.ArenaEnemies)
        .HasPermission(Permissions.Arenas.Edit);
    }
}
