using Application.Abstractions.Authorization;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Application.Game.CharacterSpecs.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.CharacterSpecs;

internal sealed class Update : IEndpoint
{
    public sealed record Request(
        string Name,
        string? Description,
        List<StatDto> Stats,
        List<ActionAssetDto> Assets
    );

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("character-specs/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateSpecCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateSpecCommand(id,
                request.Name,
                request.Description,
                request.Stats,
                request.Assets);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Enemies)
        .HasPermission(Permissions.CharacterClasses.Edit);
    }
}
