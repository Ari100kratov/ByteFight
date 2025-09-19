using Application.Abstractions.Messaging;
using Application.Game.Characters.Create;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Characters;

internal sealed class Create : IEndpoint
{
    public sealed record Request(string Name);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("characters", async (
            Request request,
            ICommandHandler<CreateCharacterCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateCharacterCommand(request.Name);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Characters)
        .RequireAuthorization();
    }
}
