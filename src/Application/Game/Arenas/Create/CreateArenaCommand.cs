using Application.Abstractions.Messaging;
using Domain.Game.GameModes;

namespace Application.Game.Arenas.Create;

public sealed record CreateArenaCommand(
    string Name,
    int GridWidth,
    int GridHeight,
    string? BackgroundAsset,
    string? Description,
    List<GameModeType> GameModes
) : ICommand<Guid>;
