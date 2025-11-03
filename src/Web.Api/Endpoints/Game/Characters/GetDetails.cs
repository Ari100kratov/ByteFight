using Application.Abstractions.Messaging;
using Application.Game.Characters.GetDetails;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Characters;

internal sealed class GetDetails : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("characters/{id:guid}/details", async (
            Guid id,
            IQueryHandler<GetCharacterDetailsQuery, CharacterResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCharacterDetailsQuery(id);

            Result<CharacterResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Characters)
        .RequireAuthorization();
    }
}
