using Application.Abstractions.Authorization;
using Application.Abstractions.Messaging;
using Application.Game.CharacterSpecs.UpdatePortrait;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.CharacterSpecs;

internal sealed class UpdatePortrait : IEndpoint
{
    public sealed record Request(Uri PortraitUrl);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("character-specs/{id:guid}/portrait", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateSpecPortraitCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateSpecPortraitCommand(id, request.PortraitUrl);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.CharacterSpecs)
        .HasPermission(Permissions.CharacterSpecs.Edit);
    }
}
