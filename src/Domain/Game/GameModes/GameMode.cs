using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Game.GameModes;

public static class GameMode
{
    public static readonly IReadOnlyList<GameModeInfo> All =
    [
        new(GameModeType.Training, "training", "Тренировка", "Испытай своего бота в разных сценариях без риска."),
        new(GameModeType.PvE, "pve", "PvE", "Сражайся против монстров на различных картах."),
        new(GameModeType.PvP, "pvp", "PvP", "Соревнуйся с другими игроками онлайн.")
    ];

    public static GameModeInfo Get(GameModeType type) =>
        All.First(m => m.Type == type);
}
