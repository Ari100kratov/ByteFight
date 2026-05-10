using System;
using System.Collections.Generic;
using System.Text;
using SharedKernel;

namespace Domain.Game.ArenaItems;

/// <summary>
/// Тип предмета на арене.
/// </summary>
[UserCodeApi]
public enum ArenaItemType
{
    /// <summary>
    /// Целебное зелье.
    /// </summary>
    HealingPotion = 1
}
