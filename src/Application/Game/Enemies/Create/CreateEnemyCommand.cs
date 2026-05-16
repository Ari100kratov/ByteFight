
using Application.Contracts;
using SharedKernel.Messaging;

namespace Application.Game.Enemies.Create;

public sealed record CreateEnemyCommand(
    string Name,
    string? Description,
    List<StatDto> Stats,
    List<ActionAssetDto> ActionAssets
) : ICommand<Guid>;
