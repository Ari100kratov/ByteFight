using Application.Abstractions.Messaging;
using Application.Contracts;

namespace Application.Game.Enemies.UpdateActionAssets;

public sealed record UpdateEnemyActionAssetsCommand(Guid Id, IReadOnlyList<ActionAssetDto> ActionAssets) : ICommand;
