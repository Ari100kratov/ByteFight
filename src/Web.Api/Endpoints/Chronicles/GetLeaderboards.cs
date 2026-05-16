using Chronicles.Application.Chronicles.GetTopNominations;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Chronicles;

internal sealed class GetLeaderboards : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("chronicles/leaderboards", async (
            int? top,
            IChroniclesLeaderboardReader reader,
            CancellationToken cancellationToken) =>
        {
            int requestedTop = Math.Clamp(top.GetValueOrDefault(10), 1, 100);

            Result<IReadOnlyList<ChronicleNominationLeaderboardResponse>> result =
                await reader.GetTopAsync(requestedTop, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Chronicles)
        .RequireAuthorization();
    }
}
