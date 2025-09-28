using Application.Abstractions.Messaging;
using Application.Auth.Users.GetCurrent;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth.Users;

internal sealed class GetCurrent : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/current", async (
            IQueryHandler<GetCurrentUserQuery, UserResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCurrentUserQuery();

            Result<UserResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Users);
    }
}
