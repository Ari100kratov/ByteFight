using Application.Abstractions.Authorization;

using Application.Contracts;
using Application.Game.CharacterSpecs.UpdateActionAssets;
using SharedKernel;
using SharedKernel.Messaging;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.CharacterSpecs;

internal sealed class UpdateActionAssets : IEndpoint
{
    public sealed record Request(IReadOnlyList<ActionAssetDto> ActionAssets);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("character-specs/{id:guid}/action-assets", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateSpecActionAssetsCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateSpecActionAssetsCommand(id, request.ActionAssets);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.CharacterSpecs)
        .HasPermission(Permissions.CharacterSpecs.Edit);
    }
}
