namespace Domain.Game.Actions;

/// <summary>
/// Тип действия или анимации юнита.
/// </summary>
public enum ActionType
{
    /// <summary>
    /// Бездействие.
    /// </summary>
    Idle = 1,

    /// <summary>
    /// Ходьба.
    /// </summary>
    Walk = 2,

    /// <summary>
    /// Бег.
    /// </summary>
    Run = 3,

    /// <summary>
    /// Атака.
    /// </summary>
    Attack = 4,

    /// <summary>
    /// Атака во время бега.
    /// </summary>
    Run_Attack = 5,

    /// <summary>
    /// Прыжок.
    /// </summary>
    Jump = 6,

    /// <summary>
    /// Получение урона.
    /// </summary>
    Hurt = 7,

    /// <summary>
    /// Смерть.
    /// </summary>
    Dead = 8,

    /// <summary>
    /// Применение способности или заклинания.
    /// </summary>
    Cast = 9
}
