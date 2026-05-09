using Application.Abstractions.Messaging;
using Application.Contracts;

namespace Application.Game.CharacterSpecs.UpdateActionAssets;

public sealed record UpdateSpecActionAssetsCommand(Guid Id, IReadOnlyList<ActionAssetDto> ActionAssets) : ICommand;
