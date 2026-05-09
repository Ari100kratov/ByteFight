using Application.Abstractions.Authorization;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Application.Game.Enemies.UpdateActionAssets;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Enemies;

internal sealed class UpdateActionAssets : IEndpoint
{
    public sealed record Request(IReadOnlyList<ActionAssetDto> ActionAssets);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("enemies/{id:guid}/action-assets", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateEnemyActionAssetsCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateEnemyActionAssetsCommand(id, request.ActionAssets);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Enemies)
        .HasPermission(Permissions.Enemies.EditActionAssets);
    }
}
