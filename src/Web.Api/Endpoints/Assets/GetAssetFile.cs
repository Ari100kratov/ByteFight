using Application.Abstractions.Messaging;
using Application.Assets.GetAssetFile;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Assets;

internal sealed class GetAssetFile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("assets/{**key}", async (
            string key,
            IQueryHandler<GetAssetFileQuery, StreamResult> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAssetFileQuery(key);
            Result<StreamResult> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                onSuccess: file => Results.File(file.Stream, file.ContentType, file.FileName),
                onFailure: CustomResults.Problem
            );
        })
        .WithTags(Tags.Assets)
        .AllowAnonymous();
    }
}
