using Application.Abstractions.Messaging;
using Application.Contracts.GameRuntime;
using Application.GameRuntime.GameSessions.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.GameRuntime.GameSessions;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("game/sessions/{id:guid}", async (
            Guid id,
            IQueryHandler<GetGameSessionByIdQuery, GameSessionDto> handler,
            CancellationToken cancellationToken) =>
        {
            Result<GameSessionDto> result = await handler.Handle(new GetGameSessionByIdQuery(id), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Game)
        .RequireAuthorization();
    }
}
