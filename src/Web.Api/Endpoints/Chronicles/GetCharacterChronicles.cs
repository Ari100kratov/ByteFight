using Chronicles.Application.Chronicles.GetCharacterChronicles;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Chronicles;

internal sealed class GetCharacterChronicles : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("chronicles/characters/{characterId:guid}", async (
            Guid characterId,
            ICharacterChroniclesReader reader,
            CancellationToken cancellationToken) =>
        {
            Result<CharacterChroniclesResponse> result = await reader.GetAsync(characterId, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Chronicles)
        .RequireAuthorization();
    }
}
