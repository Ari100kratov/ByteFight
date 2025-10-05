using Application.Abstractions.Messaging;
using Domain.Game.GameModes;
using SharedKernel;

namespace Application.Game.GameModes;

internal sealed class GetGameModesQueryHandler
    : IQueryHandler<GetGameModesQuery, IReadOnlyList<GameModeResponse>>
{
    public Task<Result<IReadOnlyList<GameModeResponse>>> Handle(
        GetGameModesQuery query,
        CancellationToken cancellationToken)
    {
        var response = GameMode.All
            .Select(m => new GameModeResponse
            {
                Id = (int)m.Type,
                Slug = m.Slug,
                Name = m.Name,
                Description = m.Description
            })
            .ToList();

        return Task.FromResult(Result.Success<IReadOnlyList<GameModeResponse>>(response));
    }
}
