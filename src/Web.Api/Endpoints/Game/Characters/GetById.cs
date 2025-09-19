using Application.Abstractions.Messaging;
using Application.Game.Characters.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Characters;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("characters/{id:guid}", async (
            Guid id,
            IQueryHandler<GetCharacterByIdQuery, CharacterResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCharacterByIdQuery(id);

            Result<CharacterResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Characters)
        .RequireAuthorization();
    }
}
