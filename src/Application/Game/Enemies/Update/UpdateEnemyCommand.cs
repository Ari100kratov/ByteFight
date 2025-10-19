using Application.Abstractions.Messaging;
using Domain.Game.Actions;
using Domain.Game.Stats;

namespace Application.Game.Enemies.Update;

public sealed record UpdateEnemyCommand(
    Guid Id,
    string Name,
    string? Description,
    List<EnemyStatDto> Stats,
    List<EnemyAssetDto> Assets
) : ICommand;

public sealed record EnemyStatDto(StatType StatType, decimal Value);
public sealed record EnemyAssetDto(ActionType ActionType, Uri Url);
