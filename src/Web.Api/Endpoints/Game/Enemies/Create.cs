using Application.Abstractions.Messaging;
using Application.Contracts;
using Application.Game.Enemies.Create;
using SharedKernel;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Game.Enemies;

internal sealed class Create : IEndpoint
{
    public sealed record Request(
        string Name,
        string? Description,
        List<StatDto> Stats,
        List<ActionAssetDto> Assets
    );

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("enemies", async (
            Request request,
            ICommandHandler<CreateEnemyCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateEnemyCommand(
                request.Name,
                request.Description,
                request.Stats,
                request.Assets);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.ToCreated(id => $"/enemies/{id}");
        })
        .WithTags(Tags.Enemies)
        .HasPermission(Permissions.Enemies.Create);
    }
}
