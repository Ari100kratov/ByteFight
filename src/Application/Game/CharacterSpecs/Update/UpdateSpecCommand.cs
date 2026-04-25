using Application.Abstractions.Messaging;
using Application.Contracts;

namespace Application.Game.CharacterSpecs.Update;

public sealed record UpdateSpecCommand(
    Guid Id,
    string Name,
    string? Description,
    List<StatDto> Stats,
    List<ActionAssetDto> ActionAssets
) : ICommand;
