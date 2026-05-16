
using Application.Contracts;
using SharedKernel.Messaging;

namespace Application.Game.Enemies.Update;

public sealed record UpdateEnemyCommand(
    Guid Id,
    string Name,
    string? Description,
    List<StatDto> Stats,
    List<ActionAssetDto> ActionAssets
) : ICommand;
