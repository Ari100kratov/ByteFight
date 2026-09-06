using Application.Game.Characters.Talents.GetTalents;
using Application.Game.Characters.Talents.Learn;
using Application.Game.Characters.Talents.Reset;
using SharedKernel;
using SharedKernel.Messaging;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Game.Characters;

internal sealed class Talents : IEndpoint
{
    public sealed record LearnRequest(string TalentId);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("characters/{id:guid}/talents", async (
            Guid id,
            IQueryHandler<GetCharacterTalentsQuery, CharacterTalentsResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<CharacterTalentsResponse> result =
                await handler.Handle(new GetCharacterTalentsQuery(id), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Characters)
        .RequireAuthorization();

        app.MapPost("characters/{id:guid}/talents", async (
            Guid id,
            LearnRequest request,
            ICommandHandler<LearnTalentCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(
                new LearnTalentCommand(id, request.TalentId),
                cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Characters)
        .RequireAuthorization();

        app.MapDelete("characters/{id:guid}/talents", async (
            Guid id,
            ICommandHandler<ResetTalentsCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new ResetTalentsCommand(id), cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Characters)
        .RequireAuthorization();
    }
}
