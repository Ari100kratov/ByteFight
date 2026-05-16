
using Application.Contracts;
using Domain.Game.GameModes;
using SharedKernel.Messaging;

namespace Application.Game.Arenas.Update;

public sealed record UpdateArenaCommand(
    Guid Id,
    string Name,
    int GridWidth,
    int GridHeight,
    string BackgroundAsset,
    Uri ImageUrl,
    string? Description,
    List<GameModeType> GameModes,
    PositionDto? StartPosition,
    List<PositionDto>? BlockedPositions,
    bool IsActive
) : ICommand;
