using Application.Abstractions.Messaging;
using Application.Game.Characters.Create;
using SharedKernel;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Game.Characters;

internal sealed class Create : IEndpoint
{
    public sealed record Request(string Name, Guid ClassId);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("characters", async (
            Request request,
            ICommandHandler<CreateCharacterCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateCharacterCommand(request.Name, request.ClassId);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.ToCreated(id => $"/characters/{id}");
        })
        .WithTags(Tags.Characters)
        .RequireAuthorization();
    }
}
