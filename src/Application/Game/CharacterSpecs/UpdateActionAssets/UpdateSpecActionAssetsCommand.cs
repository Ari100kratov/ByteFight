
using Application.Contracts;
using SharedKernel.Messaging;

namespace Application.Game.CharacterSpecs.UpdateActionAssets;

public sealed record UpdateSpecActionAssetsCommand(Guid Id, IReadOnlyList<ActionAssetDto> ActionAssets) : ICommand;
