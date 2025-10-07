namespace Domain.Game.GameModes;

public sealed record GameModeInfo(
    GameModeType Type,
    string Slug,
    string Name,
    string Description
);
