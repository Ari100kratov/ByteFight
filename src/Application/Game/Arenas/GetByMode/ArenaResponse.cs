namespace Application.Game.Arenas.GetByMode;

public sealed record ArenaResponse(
    Guid Id,
    string Name,
    Uri ImageUrl,
    int GridWidth,
    int GridHeight,
    string? Description,
    IReadOnlyList<ArenaEnemySummaryResponse> Enemies,
    IReadOnlyList<ArenaItemSummaryResponse> Items
);

public sealed record ArenaEnemySummaryResponse(
    Guid EnemyId,
    string Name,
    int Count
);

public sealed record ArenaItemSummaryResponse(
    Guid ItemId,
    string Name,
    int Count
);
