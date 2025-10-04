using Application.Abstractions.Messaging;
using Application.Game.CharacterCodes.GetByCharacterId;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Characters.Codes;

internal sealed class GetCodesByCharacterId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("characters/{id:guid}/codes", async (
            Guid id,
            IQueryHandler<GetCodesByCharacterIdQuery, IReadOnlyList<CharacterCodeResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCodesByCharacterIdQuery(id);

            Result<IReadOnlyList<CharacterCodeResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.CharacterCodes)
        .RequireAuthorization();
    }
}
