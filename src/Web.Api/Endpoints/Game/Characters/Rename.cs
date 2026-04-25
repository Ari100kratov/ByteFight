using Application.Abstractions.Messaging;
using Application.Game.Characters.Rename;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Characters;

internal sealed class Rename : IEndpoint
{
    public sealed record Request(string Name);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("characters/{id:guid}/name", async (
            Guid id,
            Request request,
            ICommandHandler<RenameCharacterCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RenameCharacterCommand(id, request.Name);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Characters)
        .RequireAuthorization();
    }
}
