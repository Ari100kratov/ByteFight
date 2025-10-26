using Application.Abstractions.Messaging;
using Application.Game.CharacterClasses.GetAll;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.CharacterClasses;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("character-classes", async (
            IQueryHandler<GetAllClassesQuery, IReadOnlyList<ClassResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllClassesQuery();

            Result<IReadOnlyList<ClassResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.CharacterClasses)
        .RequireAuthorization();
    }
}
