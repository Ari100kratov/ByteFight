using Application.Contracts;

namespace Application.Game.Enemies.GetById;

public sealed record EnemyResponse(
    Guid Id,
    string Name,
    string? Description,
    IReadOnlyList<StatDto> Stats,
    IReadOnlyList<ActionAssetDto> ActionAssets
);
