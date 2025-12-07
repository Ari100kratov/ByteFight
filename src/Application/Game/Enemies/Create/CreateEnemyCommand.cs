using Application.Abstractions.Messaging;
using Application.Contracts;

namespace Application.Game.Enemies.Create;

public sealed record CreateEnemyCommand(
    string Name,
    string? Description,
    List<StatDto> Stats,
    List<ActionAssetDto> ActionAssets
) : ICommand<Guid>;
