using Application.Abstractions.Messaging;
using Application.Game.CharacterCodes.GetTemplate;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Characters.Codes;

internal sealed class GetCodeTemplate : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("characters/codes/template", async (
            IQueryHandler<GetCodeTemplateQuery, CodeTemplateResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCodeTemplateQuery();

            Result<CodeTemplateResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.CharacterCodes)
        .RequireAuthorization();
    }
}
