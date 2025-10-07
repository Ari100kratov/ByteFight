using Application.Abstractions.Messaging;
using Application.Game.Arenas.Create;
using Domain.Game.GameModes;
using SharedKernel;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Game.Arenas;

internal sealed class Create : IEndpoint
{
    public sealed record Request(
        string Name,
        int GridWidth,
        int GridHeight,
        string? BackgroundAsset,
        string? Description,
        List<GameModeType> GameModes
    );

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("arenas", async (
            Request request,
            ICommandHandler<CreateArenaCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateArenaCommand(
                request.Name,
                request.GridWidth,
                request.GridHeight,
                request.BackgroundAsset,
                request.Description,
                request.GameModes
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.ToCreated(id => $"/arenas/{id}");
        })
        .WithTags(Tags.Arenas)
        .HasPermission(Permissions.Arenas.Create);
    }
}
