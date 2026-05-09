using Application.Abstractions.Authorization;
using Application.Abstractions.Messaging;
using Application.Game.Enemies.UpdateDescription;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Enemies;

internal sealed class UpdateDescription : IEndpoint
{
    public sealed record Request(string? Description);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("enemies/{id:guid}/description", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateEnemyDescriptionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateEnemyDescriptionCommand(id, request.Description);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Enemies)
        .HasPermission(Permissions.Enemies.EditDescription);
    }
}
