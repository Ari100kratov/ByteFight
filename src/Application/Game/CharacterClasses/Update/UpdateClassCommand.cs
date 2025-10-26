using Application.Abstractions.Messaging;
using Application.Game.Common.Dtos;

namespace Application.Game.CharacterClasses.Update;

public sealed record UpdateClassCommand(
    Guid Id,
    string Name,
    string? Description,
    List<StatDto> Stats,
    List<ActionAssetDto> ActionAssets
) : ICommand;
