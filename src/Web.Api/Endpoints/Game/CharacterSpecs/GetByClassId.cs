using Application.Abstractions.Messaging;
using Application.Game.CharacterSpecs.GetSpecsByClassId;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.CharacterSpecs;

internal sealed class GetByClassId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("character-specs", async (
            Guid classId,
            IQueryHandler<GetSpecsByClassIdQuery, IReadOnlyList<SpecResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSpecsByClassIdQuery(classId);

            Result<IReadOnlyList<SpecResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.CharacterSpecs)
        .RequireAuthorization();
    }
}
