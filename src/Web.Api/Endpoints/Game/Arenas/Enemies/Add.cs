using Application.Abstractions.Messaging;
using Application.Contracts;
using Application.Game.Arenas.Enemies.Add;
using SharedKernel;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Game.Arenas.Enemies;

internal sealed class Add : IEndpoint
{
    public sealed record Request(Guid EnemyId, PositionDto Position);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("arenas/{arenaId:guid}/enemies", async (
            Guid arenaId,
            Request request,
            ICommandHandler<AddEnemyToArenaCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AddEnemyToArenaCommand(arenaId, request.EnemyId, request.Position);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.ToCreated(id => $"/arenas/{arenaId}/enemies/{id}");
        })
        .WithTags(Tags.ArenaEnemies)
        .HasPermission(Permissions.Arenas.Edit);
    }
}
