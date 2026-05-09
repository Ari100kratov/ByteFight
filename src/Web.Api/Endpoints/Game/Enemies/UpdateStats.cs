using Application.Abstractions.Authorization;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Application.Game.Enemies.UpdateStats;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Enemies;

internal sealed class UpdateStats : IEndpoint
{
    public sealed record Request(IReadOnlyList<StatDto> Stats);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("enemies/{id:guid}/stats", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateEnemyStatsCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateEnemyStatsCommand(id, request.Stats);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Enemies)
        .HasPermission(Permissions.Enemies.EditStats);
    }
}
