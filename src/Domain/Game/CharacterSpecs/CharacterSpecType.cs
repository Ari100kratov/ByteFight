namespace Domain.Game.CharacterSpecs;

/// <summary>
/// Специализация (стезя) персонажа внутри класса.
/// Значения 10+ — действующая древнеславянская линейка.
/// </summary>
public enum CharacterSpecType
{
    Berserker = 1,
    Guardian = 2,
    Duelist = 3,

    Pyromancer = 100,
    Luminary = 101,
    Arcanist = 102,

    /// <summary>Мечник — витязь, сделавший ставку на атаку.</summary>
    SwordBearer = 10,

    /// <summary>Щитоносец — витязь-защитник.</summary>
    ShieldBearer = 11,

    /// <summary>Дружинник — витязь, равномерно развитый в бою.</summary>
    Druzhinnik = 12,

    /// <summary>Громовник — волхв повелевает грозой.</summary>
    StormCaller = 20,

    /// <summary>Обережник — волхв, хранящий союзников.</summary>
    WardKeeper = 21,

    /// <summary>Ведун — волхв, уравновешенный в стихиях.</summary>
    Veden = 22,

    /// <summary>Лучник — охотник, владеющий дальним боем.</summary>
    Archer = 30,

    /// <summary>Ловчий — охотник на силках и капканах.</summary>
    Trapper = 31,

    /// <summary>Зверолов — охотник, чтящий дух зверя.</summary>
    BeastStalker = 32,

    /// <summary>Травница — ведунья-целительница.</summary>
    Herbalist = 40,

    /// <summary>Шептунья — ведунья порчи и проклятий.</summary>
    Whisperer = 41,

    /// <summary>Ворожея — ведунья равновесия.</summary>
    Sorceress = 42
}
