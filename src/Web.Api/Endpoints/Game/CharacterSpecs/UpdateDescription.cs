using Application.Abstractions.Authorization;
using Application.Abstractions.Messaging;
using Application.Game.CharacterSpecs.UpdateDescription;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.CharacterSpecs;

internal sealed class UpdateDescription : IEndpoint
{
    public sealed record Request(string? Description);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("character-specs/{id:guid}/description", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateSpecDescriptionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateSpecDescriptionCommand(id, request.Description);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.CharacterSpecs)
        .HasPermission(Permissions.CharacterSpecs.EditDescription);
    }
}
