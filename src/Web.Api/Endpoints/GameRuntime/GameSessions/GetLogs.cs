using Application.Abstractions.Messaging;
using Application.Contracts.GameRuntime;
using Application.GameRuntime.GameSessions.GetLogs;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.GameRuntime.GameSessions;

internal sealed class GetLogs : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("game/sessions/{id:guid}/logs", async (
            Guid id,
            IQueryHandler<GetGameSessionLogsQuery, IReadOnlyList<TurnLogDto>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<TurnLogDto>> result =
                await handler.Handle(new GetGameSessionLogsQuery(id), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.GameSessions)
        .RequireAuthorization();
    }
}
