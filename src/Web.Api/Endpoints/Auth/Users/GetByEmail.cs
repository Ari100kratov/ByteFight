using Application.Abstractions.Messaging;
using Application.Auth.Users.GetByEmail;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth.Users;

internal sealed class GetByEmail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/by-email", async (
            [FromQuery] string email,
            IQueryHandler<GetUserByEmailQuery, UserResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserByEmailQuery(email);

            Result<UserResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(Permissions.Users.Access)
        .WithTags(Tags.Users);
    }
}
