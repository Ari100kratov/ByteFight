using Application.Abstractions.Messaging;
using Application.Game.Characters.GetByUserId;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Characters;

internal sealed class GetByUserId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("characters/by-user/{userId:guid}", async (
            Guid userId,
            IQueryHandler<GetCharactersByUserIdQuery, IReadOnlyList<CharacterResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCharactersByUserIdQuery(userId);

            Result<IReadOnlyList<CharacterResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Characters)
        .RequireAuthorization();
    }
}
