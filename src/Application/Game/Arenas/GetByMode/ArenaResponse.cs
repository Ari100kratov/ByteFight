namespace Application.Game.Arenas.GetByMode;

public sealed record ArenaResponse(
    Guid Id,
    string Name,
    int GridWidth,
    int GridHeight,
    string? Description
);
