using Application.Abstractions.Authorization;

using Application.Contracts;
using Application.Game.Enemies.UpdateAbilities;
using SharedKernel;
using SharedKernel.Messaging;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Enemies;

internal sealed class UpdateAbilities : IEndpoint
{
    public sealed record Request(IReadOnlyList<AbilityDto> Abilities);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("enemies/{id:guid}/abilities", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateEnemyAbilitiesCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateEnemyAbilitiesCommand(id, request.Abilities);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Enemies)
        .HasPermission(Permissions.Enemies.Edit);
    }
}
