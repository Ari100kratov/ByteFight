using Application.Abstractions.Messaging;
using Domain.Game.Actions;
using Domain.Game.Stats;

namespace Application.Game.Enemies.Create;

public sealed record CreateEnemyCommand(
    string Name,
    string? Description,
    List<EnemyStatDto> Stats,
    List<EnemyAssetDto> Assets
) : ICommand<Guid>;

public sealed record EnemyStatDto(StatType StatType, decimal Value);
public sealed record EnemyAssetDto(ActionType ActionType, Uri Url);
