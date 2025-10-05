using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Game.GameModes;

public sealed record GameModeInfo(
    GameModeType Type,
    string Slug,
    string Name,
    string Description
);
