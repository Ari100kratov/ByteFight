using Application.Abstractions.Messaging;
using Application.Contracts;
using Domain.Game.GameModes;

namespace Application.Game.Arenas.Create;

public sealed record CreateArenaCommand(
    string Name,
    int GridWidth,
    int GridHeight,
    string? BackgroundAsset,
    string? Description,
    List<GameModeType> GameModes,
    PositionDto? StartPosition,
    List<PositionDto>? BlockedPositions
) : ICommand<Guid>;
