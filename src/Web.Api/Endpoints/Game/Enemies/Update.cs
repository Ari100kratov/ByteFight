using Application.Abstractions.Messaging;
using Application.Game.Common.Dtos;
using Application.Game.Enemies.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Enemies;

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
        app.MapPut("enemies/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateEnemyCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateEnemyCommand(id,
                request.Name,
                request.Description,
                request.Stats,
                request.Assets);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Enemies)
        .HasPermission(Permissions.Enemies.Edit);
    }
}
