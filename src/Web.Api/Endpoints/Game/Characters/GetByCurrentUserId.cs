using Application.Abstractions.Messaging;
using Application.Game.Characters.GetByCurrentUserId;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Characters;

internal sealed class GetByCurrentUserId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("characters/by-current-user", async (
            IQueryHandler<GetCharactersByCurrentUserIdQuery, IReadOnlyList<CharacterResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCharactersByCurrentUserIdQuery();

            Result<IReadOnlyList<CharacterResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Characters)
        .RequireAuthorization();
    }
}
