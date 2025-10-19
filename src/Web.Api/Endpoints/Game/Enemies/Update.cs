using Application.Abstractions.Messaging;
using Application.Game.Enemies.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Enemies;

internal sealed class Update : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("enemies/{id:guid}", async (
            Guid id,
            UpdateEnemyCommand command,
            ICommandHandler<UpdateEnemyCommand> handler,
            CancellationToken cancellationToken) =>
        {
            UpdateEnemyCommand updatedCommand = command with { Id = id };
            Result result = await handler.Handle(updatedCommand, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Enemies)
        .HasPermission(Permissions.Enemies.Edit);
    }
}
