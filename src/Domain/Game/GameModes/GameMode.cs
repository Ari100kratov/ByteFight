namespace Domain.Game.GameModes;

public static class GameMode
{
    public static readonly IReadOnlyList<GameModeInfo> All =
    [
        new(
            GameModeType.Training,
            "training",
            "Тренировка",
            new Uri("game-mode-cards/training.png", UriKind.Relative),
            "Испытай своего бота в разных сценариях без риска."),
        new(
            GameModeType.PvE,
            "pve",
            "PvE",
            new Uri("game-mode-cards/pve.png", UriKind.Relative),
            "Сражайся против монстров на различных аренах."),
        new(
            GameModeType.PvP,
            "pvp",
            "PvP",
            new Uri("game-mode-cards/pvp.png", UriKind.Relative),
            "Соревнуйся с другими игроками онлайн.")
    ];

    public static GameModeInfo Get(GameModeType type) =>
        All.First(m => m.Type == type);
}
