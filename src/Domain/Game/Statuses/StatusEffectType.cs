using SharedKernel;

namespace Domain.Game.Statuses;

/// <summary>
/// Тип статус-эффекта, накладываемого способностью.
/// </summary>
[UserCodeApi]
public enum StatusEffectType
{
    /// <summary>
    /// Горение: магический урон в начале хода носителя.
    /// </summary>
    Burn = 1,

    /// <summary>
    /// Яд: урон в начале хода носителя.
    /// </summary>
    Poison = 2,

    /// <summary>
    /// Регенерация: восстановление здоровья в начале хода носителя.
    /// </summary>
    Regeneration = 3,

    /// <summary>
    /// Щит: поглощает входящий урон, пока пул не иссякнет.
    /// </summary>
    Shield = 4,

    /// <summary>
    /// Замедление: очки перемещения снижены на указанную величину.
    /// </summary>
    Slow = 5,

    /// <summary>
    /// Оковы корней: перемещение невозможно.
    /// </summary>
    Root = 6,

    /// <summary>
    /// Оглушение: пропуск хода целиком.
    /// </summary>
    Stun = 7,

    /// <summary>
    /// Немощь: исходящий урон снижен на указанную величину.
    /// </summary>
    Weaken = 8,

    /// <summary>
    /// Мощь: исходящий урон увеличен на указанную величину.
    /// </summary>
    Might = 9,

    /// <summary>
    /// Оберег: входящий урон снижен на указанный процент.
    /// </summary>
    Ward = 10
}
