using GameRuntime.UserCodeApiDocumentation;

namespace Web.Api.Endpoints.Documentation;

internal sealed class GetUserCodeApiDocumentation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("docs/user-code-api", (UserCodeApiDocSnapshot snapshot) =>
        {
            return Results.Ok(snapshot.Value);
        })
        .WithTags(Tags.Documentation)
        .RequireAuthorization();
    }
}
