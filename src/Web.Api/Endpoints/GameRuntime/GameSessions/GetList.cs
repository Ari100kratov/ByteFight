using Application.Abstractions.Messaging;
using Application.Contracts;
using Application.GameRuntime.GameSessions.GetList;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.GameRuntime.GameSessions;

internal sealed class GetList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("game/sessions", async (
            int? page,
            int? pageSize,
            IQueryHandler<GetGameSessionsQuery, PagedResponse<GameSessionListItemDto>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<PagedResponse<GameSessionListItemDto>> result = await handler.Handle(
                new GetGameSessionsQuery(
                    page ?? 1,
                    pageSize ?? 20),
                cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.GameSessions)
        .RequireAuthorization();
    }
}
