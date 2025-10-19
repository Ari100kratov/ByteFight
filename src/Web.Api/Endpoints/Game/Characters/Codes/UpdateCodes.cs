using Application.Abstractions.Messaging;
using Application.Game.Characters.CharacterCodes.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Characters.Codes;

internal sealed class UpdateCodes : IEndpoint
{
    public sealed record Request(
        List<CharacterCodeDto> Created,
        List<CharacterCodeDto> Updated,
        List<Guid> DeletedIds
    );

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("characters/{id:guid}/codes", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateCodesCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateCodesCommand(
                id,
                request.Created ?? [],
                request.Updated ?? [],
                request.DeletedIds ?? []
            );

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.CharacterCodes)
        .RequireAuthorization();
    }
}
