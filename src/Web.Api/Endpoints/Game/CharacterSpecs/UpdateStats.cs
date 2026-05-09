using Application.Abstractions.Authorization;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Application.Game.CharacterSpecs.UpdateStats;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.CharacterSpecs;

internal sealed class UpdateStats : IEndpoint
{
    public sealed record Request(IReadOnlyList<StatDto> Stats);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("character-specs/{id:guid}/stats", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateSpecStatsCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateSpecStatsCommand(id, request.Stats);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.CharacterSpecs)
        .HasPermission(Permissions.CharacterSpecs.Edit);
    }
}
