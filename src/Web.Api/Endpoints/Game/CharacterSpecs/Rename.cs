using Application.Abstractions.Authorization;

using Application.Game.CharacterSpecs.Rename;
using SharedKernel;
using SharedKernel.Messaging;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.CharacterSpecs;

internal sealed class Rename : IEndpoint
{
    public sealed record Request(string Name);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("character-specs/{id:guid}/name", async (
            Guid id,
            Request request,
            ICommandHandler<RenameSpecCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RenameSpecCommand(id, request.Name);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.CharacterSpecs)
        .HasPermission(Permissions.CharacterSpecs.Edit);
    }
}
