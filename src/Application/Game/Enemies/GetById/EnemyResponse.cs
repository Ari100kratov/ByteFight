using Domain.Game.Actions;
using Domain.Game.Stats;

namespace Application.Game.Enemies.GetById;

public sealed record EnemyResponse(
    Guid Id,
    string Name,
    string? Description,
    IReadOnlyList<EnemyStatDto> Stats,
    IReadOnlyList<EnemyAssetDto> Assets
);

public sealed record EnemyStatDto(StatType StatType, decimal Value);
public sealed record EnemyAssetDto(ActionType ActionType, Uri Url);
