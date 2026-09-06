using SharedKernel;

namespace Domain.Game.Abilities;

/// <summary>
/// Тип игровой способности юнита.
/// Базовые атаки (значения 1–3) доступны всем, остальные
/// привязаны к классам, талантам или врагам.
/// </summary>
[UserCodeApi]
public enum AbilityType
{
    /// <summary>
    /// Базовая ближняя атака.
    /// </summary>
    BasicMeleeAttack = 1,

    /// <summary>
    /// Базовая дальняя атака.
    /// </summary>
    BasicRangedAttack = 2,

    /// <summary>
    /// Способность лечения.
    /// </summary>
    Healing = 3,

    // ===== Витязь — ратник дальней дороги =====

    /// <summary>Рубящий удар мечом (ближний бой).</summary>
    VityazSwordStrike = 10,

    /// <summary>Удар щитом: урон и оглушение на один ход.</summary>
    VityazShieldBash = 11,

    /// <summary>Богатырский рывок: бросок к цели с уроном и немощью.</summary>
    VityazCharge = 12,

    /// <summary>Стойка богатыря: щит и мощь на несколько ходов.</summary>
    VityazBulwark = 13,

    /// <summary>Гнев Перуна (талант): клин молний перед собой.</summary>
    VityazPerunWrath = 14,

    // ===== Волхв — хранитель вед =====

    /// <summary>Молния Перуна: линия урона насквозь.</summary>
    VolkhvPerunBolt = 20,

    /// <summary>Морозная длань: замедление в радиусе.</summary>
    VolkhvFrostGrip = 21,

    /// <summary>Оберег света: щит союзнику.</summary>
    VolkhvWard = 22,

    /// <summary>Пепельный туман: урон и немощь в радиусе.</summary>
    VolkhvAshCloud = 23,

    /// <summary>Столб Сварожья (талант): мощный урон в радиусе и горение.</summary>
    VolkhvSvarogPillar = 24,

    // ===== Охотник — ловчий чащоб =====

    /// <summary>Меткий выстрел: сильный урон с дальних дистанций.</summary>
    HunterPreciseShot = 30,

    /// <summary>Ловчая сеть: сковывает цель корнями на месте.</summary>
    HunterNetTrap = 31,

    /// <summary>Ядовитая стрела: урон и продолжительный яд.</summary>
    HunterPoisonArrow = 32,

    /// <summary>Маскировка: оберег и уклонение среди чащи.</summary>
    HunterCamouflage = 33,

    /// <summary>Град стрел (талант): залп по области.</summary>
    HunterVolley = 34,

    // ===== Ведунья — травница-шептунья =====

    /// <summary>Целебный отвар: восстановление здоровья союзнику.</summary>
    VedunyaHerbalBrew = 40,

    /// <summary>Порча: урон и немощь на цель.</summary>
    VedunyaHex = 41,

    /// <summary>Терновый круг: урон и замедление в радиусе.</summary>
    VedunyaThorns = 42,

    /// <summary>Дух-оберег: щит и регенерация союзнику.</summary>
    VedunyaSpiritWard = 43,

    /// <summary>Медвежий дух (талант): мощь и регенерация себе.</summary>
    VedunyaBearSpirit = 44,

    // ===== Способности врагов =====

    /// <summary>Упырь: рваные когти.</summary>
    GhoulClawStrike = 200,

    /// <summary>Упырь: поганое бешенство — мощь ценой щита.</summary>
    GhoulFrenzy = 201,

    /// <summary>Леший: корни-оковы.</summary>
    LeshyRootVines = 210,

    /// <summary>Леший: сок земли — регенерация.</summary>
    LeshyRegeneration = 211,

    /// <summary>Кикимора: ядовитый плевок.</summary>
    KikimoraPoisonSpit = 220,

    /// <summary>Волколак: прыжок с укусом.</summary>
    WolfDashBite = 230,

    /// <summary>Скелет-ратник: удар костяным мечом.</summary>
    SkeletonBoneSlash = 240,

    /// <summary>Болотный дух: миазмы порчи.</summary>
    BogCurseMiasma = 250,

    /// <summary>Упырь-князь: высасывание жизни.</summary>
    VampireDrain = 260
}
