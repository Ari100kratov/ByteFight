using Application.Abstractions.Authorization;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Application.Game.CharacterSpecs.UpdateAbilities;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.CharacterSpecs;

internal sealed class UpdateAbilities : IEndpoint
{
    public sealed record Request(IReadOnlyList<AbilityDto> Abilities);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("character-specs/{id:guid}/abilities", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateSpecAbilitiesCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateSpecAbilitiesCommand(id, request.Abilities);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.CharacterSpecs)
        .HasPermission(Permissions.CharacterSpecs.EditAbilities);
    }
}
