using Application.Abstractions.Data;
using Domain.Game.Abilities;
using Domain.Game.CharacterClasses;
using Domain.Game.CharacterSpecAbilities;
using Domain.Game.CharacterSpecs;
using Domain.Game.Stats;

namespace Infrastructure.Database.Seed.GameDataSeeders;

/// <summary>
/// Древнеславянские классы и их стези. Игровые значения способностей
/// берутся из <see cref="AbilityCatalog"/>; здесь фиксируется состав
/// способностей стези и базовые характеристики.
/// </summary>
internal static class CharacterClassesSeeder
{
    public static void Seed(SeedContext seed, IGameDbContext dbContext)
    {
        CharacterClass vityaz = CreateVityaz(seed);
        CharacterClass volkhv = CreateVolkhv(seed);
        CharacterClass okhotnik = CreateOkhotnik(seed);
        CharacterClass vedunya = CreateVedunya(seed);

        dbContext.CharacterClasses.AddRange(vityaz, volkhv, okhotnik, vedunya);
    }

    // ===== Витязь =====

    private static CharacterClass CreateVityaz(SeedContext seed)
    {
        CharacterClass vityaz = new()
        {
            Id = Guid.CreateVersion7(),
            Type = CharacterClassType.Vityaz,
            Name = "Витязь",
            Description = "Ратник дальней дороги: меч, щит и богатырская стойкость. " +
                "Держит строй, ломает строй чужой."
        };

        CharacterSpec swordBearer = CreateSpec(
            vityaz,
            CharacterSpecType.SwordBearer,
            "Мечник",
            "Витязь, сделавший ставку на атаку: бьёт часто и больно.",
            health: 190, mana: 40, manaRegen: 6, moveRange: 3, initiative: 8, armor: 4, power: 10,
            abilities:
            [
                AbilityType.BasicMeleeAttack,
                AbilityType.VityazSwordStrike,
                AbilityType.VityazCharge,
                AbilityType.VityazBulwark
            ],
            id => seed.Spec_Vityaz_SwordBearer = id);

        CharacterSpec shieldBearer = CreateSpec(
            vityaz,
            CharacterSpecType.ShieldBearer,
            "Щитоносец",
            "Защитник рода: терпелив, бронирован и очень убедителен щитом.",
            health: 240, mana: 40, manaRegen: 6, moveRange: 2, initiative: 6, armor: 8, power: 5,
            abilities:
            [
                AbilityType.BasicMeleeAttack,
                AbilityType.VityazSwordStrike,
                AbilityType.VityazShieldBash,
                AbilityType.VityazBulwark
            ],
            id => seed.Spec_Vityaz_ShieldBearer = id);

        CharacterSpec druzhinnik = CreateSpec(
            vityaz,
            CharacterSpecType.Druzhinnik,
            "Дружинник",
            "Ровный боец: и ударит, и прикроет, и не потеряет головы.",
            health: 210, mana: 45, manaRegen: 7, moveRange: 3, initiative: 7, armor: 6, power: 8,
            abilities:
            [
                AbilityType.BasicMeleeAttack,
                AbilityType.VityazSwordStrike,
                AbilityType.VityazShieldBash,
                AbilityType.VityazCharge
            ],
            id => seed.Spec_Vityaz_Druzhinnik = id);

        vityaz.Specs = [swordBearer, shieldBearer, druzhinnik];
        seed.Class_Vityaz = vityaz.Id;

        return vityaz;
    }

    // ===== Волхв =====

    private static CharacterClass CreateVolkhv(SeedContext seed)
    {
        CharacterClass volkhv = new()
        {
            Id = Guid.CreateVersion7(),
            Type = CharacterClassType.Volkhv,
            Name = "Волхв",
            Description = "Хранитель вед: повелевает грозой Перуна, хладом и оберегами света."
        };

        CharacterSpec stormCaller = CreateSpec(
            volkhv,
            CharacterSpecType.StormCaller,
            "Громовник",
            "Волхв грозы: молнии прошивают строй врагов насквозь.",
            health: 130, mana: 110, manaRegen: 12, moveRange: 3, initiative: 10, armor: 1, power: 14,
            abilities:
            [
                AbilityType.BasicRangedAttack,
                AbilityType.VolkhvPerunBolt,
                AbilityType.VolkhvFrostGrip,
                AbilityType.VolkhvAshCloud
            ],
            id => seed.Spec_Volkhv_StormCaller = id);

        CharacterSpec wardKeeper = CreateSpec(
            volkhv,
            CharacterSpecType.WardKeeper,
            "Обережник",
            "Волхв-хранитель: щиты, хлад и свет на страже союзников.",
            health: 150, mana: 120, manaRegen: 13, moveRange: 3, initiative: 8, armor: 2, power: 10,
            abilities:
            [
                AbilityType.BasicRangedAttack,
                AbilityType.VolkhvPerunBolt,
                AbilityType.VolkhvFrostGrip,
                AbilityType.VolkhvWard
            ],
            id => seed.Spec_Volkhv_WardKeeper = id);

        CharacterSpec veden = CreateSpec(
            volkhv,
            CharacterSpecType.Veden,
            "Ведун",
            "Уравновешен в стихиях: и гром, и щит, и хлад — по надобности.",
            health: 140, mana: 115, manaRegen: 12, moveRange: 3, initiative: 9, armor: 1, power: 12,
            abilities:
            [
                AbilityType.BasicRangedAttack,
                AbilityType.VolkhvPerunBolt,
                AbilityType.VolkhvAshCloud,
                AbilityType.VolkhvWard
            ],
            id => seed.Spec_Volkhv_Veden = id);

        volkhv.Specs = [stormCaller, wardKeeper, veden];
        seed.Class_Volkhv = volkhv.Id;

        return volkhv;
    }

    // ===== Охотник =====

    private static CharacterClass CreateOkhotnik(SeedContext seed)
    {
        CharacterClass okhotnik = new()
        {
            Id = Guid.CreateVersion7(),
            Type = CharacterClassType.Okhotnik,
            Name = "Охотник",
            Description = "Ловчий чащоб: дальний лук, силки и стрелы с гнилым наконечником."
        };

        CharacterSpec archer = CreateSpec(
            okhotnik,
            CharacterSpecType.Archer,
            "Лучник",
            "Мастер дальнего боя: меткий выстрел решает спор до его начала.",
            health: 140, mana: 70, manaRegen: 9, moveRange: 3, initiative: 11, armor: 1, power: 12,
            abilities:
            [
                AbilityType.BasicRangedAttack,
                AbilityType.HunterPreciseShot,
                AbilityType.HunterPoisonArrow,
                AbilityType.HunterCamouflage
            ],
            id => seed.Spec_Okhotnik_Archer = id);

        CharacterSpec trapper = CreateSpec(
            okhotnik,
            CharacterSpecType.Trapper,
            "Ловчий",
            "Мастер силков: держит врага на месте, пока стрелы работают.",
            health: 150, mana: 75, manaRegen: 9, moveRange: 4, initiative: 10, armor: 2, power: 10,
            abilities:
            [
                AbilityType.BasicRangedAttack,
                AbilityType.HunterPreciseShot,
                AbilityType.HunterNetTrap,
                AbilityType.HunterCamouflage
            ],
            id => seed.Spec_Okhotnik_Trappers = id);

        CharacterSpec beastStalker = CreateSpec(
            okhotnik,
            CharacterSpecType.BeastStalker,
            "Зверолов",
            "Чтит дух зверя: подвижен, вынослив и точен.",
            health: 160, mana: 70, manaRegen: 9, moveRange: 4, initiative: 10, armor: 2, power: 11,
            abilities:
            [
                AbilityType.BasicRangedAttack,
                AbilityType.HunterPreciseShot,
                AbilityType.HunterNetTrap,
                AbilityType.HunterPoisonArrow
            ],
            id => seed.Spec_Okhotnik_BeastStalker = id);

        okhotnik.Specs = [archer, trapper, beastStalker];
        seed.Class_Okhotnik = okhotnik.Id;

        return okhotnik;
    }

    // ===== Ведунья =====

    private static CharacterClass CreateVedunya(SeedContext seed)
    {
        CharacterClass vedunya = new()
        {
            Id = Guid.CreateVersion7(),
            Type = CharacterClassType.Vedunya,
            Name = "Ведунья",
            Description = "Травница-шептунья: отвары лечат, порча калечит, духи берегут."
        };

        CharacterSpec herbalist = CreateSpec(
            vedunya,
            CharacterSpecType.Herbalist,
            "Травница",
            "Целительница: отвар возвращает союзников в строй.",
            health: 135, mana: 115, manaRegen: 13, moveRange: 3, initiative: 9, armor: 1, power: 12,
            abilities:
            [
                AbilityType.Healing,
                AbilityType.VedunyaHerbalBrew,
                AbilityType.VedunyaSpiritWard,
                AbilityType.VedunyaThorns
            ],
            id => seed.Spec_Vedunya_Herbalist = id);

        CharacterSpec whisperer = CreateSpec(
            vedunya,
            CharacterSpecType.Whisperer,
            "Шептунья",
            "Мастер порчи: враг чахнет, союзники крепнут.",
            health: 130, mana: 110, manaRegen: 12, moveRange: 3, initiative: 10, armor: 1, power: 13,
            abilities:
            [
                AbilityType.BasicRangedAttack,
                AbilityType.VedunyaHex,
                AbilityType.VedunyaThorns,
                AbilityType.VedunyaHerbalBrew
            ],
            id => seed.Spec_Vedunya_Whisperer = id);

        CharacterSpec sorceress = CreateSpec(
            vedunya,
            CharacterSpecType.Sorceress,
            "Ворожея",
            "Равновесие вед: и лечит, и портит, и читает следы.",
            health: 140, mana: 112, manaRegen: 12, moveRange: 3, initiative: 9, armor: 1, power: 12,
            abilities:
            [
                AbilityType.BasicRangedAttack,
                AbilityType.VedunyaHerbalBrew,
                AbilityType.VedunyaHex,
                AbilityType.VedunyaSpiritWard
            ],
            id => seed.Spec_Vedunya_Sorceress = id);

        vedunya.Specs = [herbalist, whisperer, sorceress];
        seed.Class_Vedunya = vedunya.Id;

        return vedunya;
    }

    private static CharacterSpec CreateSpec(
        CharacterClass characterClass,
        CharacterSpecType type,
        string name,
        string description,
        int health,
        int mana,
        int manaRegen,
        int moveRange,
        int initiative,
        int armor,
        int power,
        AbilityType[] abilities,
        Action<Guid> specIdSetter)
    {
        CharacterSpec spec = new()
        {
            Id = Guid.CreateVersion7(),
            ClassId = characterClass.Id,
            Type = type,
            Name = name,
            PortraitUrl = $"classes/{characterClass.Type}/{type}/portrait.png",
            Description = description,
            Stats =
            [
                CreateStat(StatType.Health, health),
                CreateStat(StatType.Mana, mana),
                CreateStat(StatType.ManaRegen, manaRegen),
                CreateStat(StatType.MoveRange, moveRange),
                CreateStat(StatType.Initiative, initiative),
                CreateStat(StatType.Armor, armor),
                CreateStat(StatType.Power, power)
            ],
            ActionAssets = [],
            Abilities = [.. abilities.Select((ability, index) => CreateAbility(ability, index))]
        };

        specIdSetter(spec.Id);

        return spec;
    }

    private static CharacterSpecAbility CreateAbility(AbilityType type, int index)
    {
        AbilityDefinition definition = AbilityCatalog.Get(type);

        return new CharacterSpecAbility
        {
            Id = Guid.CreateVersion7(),
            Type = definition.Type,
            EffectType = definition.EffectType,
            TargetType = definition.TargetType,
            Name = definition.Name,
            Description = definition.Description,
            Priority = index,
            Stats = [],
            ActionAssets = []
        };
    }

    private static CharacterSpecStat CreateStat(StatType type, int value) =>
        new()
        {
            StatType = type,
            Value = value
        };
}
