using Application.Abstractions.Messaging;
using Application.Game.Arenas.Update;
using Domain.Game.GameModes;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Arenas;

internal sealed class Update : IEndpoint
{
    public sealed record Request(
        string Name,
        int GridWidth,
        int GridHeight,
        string? BackgroundAsset,
        string? Description,
        List<GameModeType> GameModes,
        bool IsActive
    );

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("arenas/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateArenaCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateArenaCommand(
                id,
                request.Name,
                request.GridWidth,
                request.GridHeight,
                request.BackgroundAsset,
                request.Description,
                request.GameModes,
                request.IsActive
            );

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Arenas)
        .HasPermission(Permissions.Arenas.Edit);
    }
}
