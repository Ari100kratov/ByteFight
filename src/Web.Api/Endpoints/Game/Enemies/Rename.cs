using Application.Abstractions.Authorization;
using Application.Abstractions.Messaging;
using Application.Game.Enemies.Rename;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Enemies;

internal sealed class Rename : IEndpoint
{
    public sealed record Request(string Name);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("enemies/{id:guid}/name", async (
            Guid id,
            Request request,
            ICommandHandler<RenameEnemyCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RenameEnemyCommand(id, request.Name);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Enemies)
        .HasPermission(Permissions.Enemies.Edit);
    }
}
