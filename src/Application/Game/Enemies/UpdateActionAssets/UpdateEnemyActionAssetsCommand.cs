
using Application.Contracts;
using SharedKernel.Messaging;

namespace Application.Game.Enemies.UpdateActionAssets;

public sealed record UpdateEnemyActionAssetsCommand(Guid Id, IReadOnlyList<ActionAssetDto> ActionAssets) : ICommand;
