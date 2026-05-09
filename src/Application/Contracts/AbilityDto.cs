using Domain.Game.Abilities;
using Domain.Game.CharacterSpecAbilities;
using Domain.Game.Enemies;
using FluentValidation;

namespace Application.Contracts;

public sealed record AbilityDto(
    AbilityType Type,
    AbilityEffectType EffectType,
    AbilityTargetType TargetType,
    string Name,
    string? Description,
    int Priority,
    IReadOnlyList<AbilityStatDto> Stats,
    IReadOnlyList<ActionAssetDto> ActionAssets);

public sealed record AbilityStatDto(
    AbilityStatType StatType,
    decimal Value);

internal static partial class Mapper
{
    public static AbilityDto ToDto(this CharacterSpecAbility ability)
        => new(
            ability.Type,
            ability.EffectType,
            ability.TargetType,
            ability.Name,
            ability.Description,
            ability.Priority,
            [.. ability.Stats.Select(x => x.ToDto())],
            [.. ability.ActionAssets.Select(x => x.ToDto())]);


    public static CharacterSpecAbility ToCharacterSpecAbility(this AbilityDto dto, Guid characterSpecId)
    {
        Guid abilityId = Guid.CreateVersion7();

        return new CharacterSpecAbility
        {
            Id = abilityId,
            CharacterSpecId = characterSpecId,
            Type = dto.Type,
            EffectType = dto.EffectType,
            TargetType = dto.TargetType,
            Name = dto.Name,
            Description = dto.Description,
            Priority = dto.Priority,
            Stats = [.. dto.Stats.Select(x => x.ToCharacterSpecAbilityStat(abilityId))],
            ActionAssets = [.. dto.ActionAssets.Select(x => x.ToCharacterSpecAbilityActionAsset(abilityId))]
        };
    }

    public static CharacterSpecAbilityStat ToCharacterSpecAbilityStat(this AbilityStatDto dto, Guid abilityId)
        => new()
        {
            CharacterSpecAbilityId = abilityId,
            StatType = dto.StatType,
            Value = dto.Value
        };

    public static CharacterSpecAbilityActionAsset ToCharacterSpecAbilityActionAsset(this ActionAssetDto dto, Guid abilityId)
        => new()
        {
            CharacterSpecAbilityId = abilityId,
            ActionType = dto.ActionType,
            Variant = dto.Variant,
            Animation = dto.SpriteAnimation.ToValueObject()
        };


    public static EnemyAbility ToEnemyAbility(this AbilityDto dto, Guid enemyId)
    {
        Guid abilityId = Guid.CreateVersion7();

        return new EnemyAbility
        {
            Id = abilityId,
            EnemyId = enemyId,
            Type = dto.Type,
            EffectType = dto.EffectType,
            TargetType = dto.TargetType,
            Name = dto.Name,
            Description = dto.Description,
            Priority = dto.Priority,
            Stats = [.. dto.Stats.Select(x => x.ToEnemyAbilityStat(abilityId))],
            ActionAssets = [.. dto.ActionAssets.Select(x => x.ToEnemyAbilityActionAsset(abilityId))]
        };
    }

    public static EnemyAbilityStat ToEnemyAbilityStat(this AbilityStatDto dto, Guid abilityId)
        => new()
        {
            EnemyAbilityId = abilityId,
            StatType = dto.StatType,
            Value = dto.Value
        };

    public static EnemyAbilityActionAsset ToEnemyAbilityActionAsset(this ActionAssetDto dto, Guid abilityId)
        => new()
        {
            EnemyAbilityId = abilityId,
            ActionType = dto.ActionType,
            Variant = dto.Variant,
            Animation = dto.SpriteAnimation.ToValueObject()
        };

    public static AbilityDto ToDto(this EnemyAbility ability)
        => new(
            ability.Type,
            ability.EffectType,
            ability.TargetType,
            ability.Name,
            ability.Description,
            ability.Priority,
            [.. ability.Stats.Select(x => x.ToDto())],
            [.. ability.ActionAssets.Select(x => x.ToDto())]);

    public static AbilityStatDto ToDto(this CharacterSpecAbilityStat stat)
        => new(stat.StatType, stat.Value);

    public static AbilityStatDto ToDto(this EnemyAbilityStat stat)
        => new(stat.StatType, stat.Value);

    public static ActionAssetDto ToDto(this CharacterSpecAbilityActionAsset asset)
        => new(asset.ActionType, asset.Variant, asset.Animation.ToDto());

    public static ActionAssetDto ToDto(this EnemyAbilityActionAsset asset)
        => new(asset.ActionType, asset.Variant, asset.Animation.ToDto());
}


internal sealed class AbilityDtoValidator : AbstractValidator<AbilityDto>
{
    public AbilityDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .MaximumLength(64);

        RuleFor(x => x.Description)
            .MaximumLength(512);

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Stats)
            .NotNull();

        RuleForEach(x => x.Stats)
            .SetValidator(new AbilityStatDtoValidator());

        RuleFor(x => x.ActionAssets)
            .NotNull();

        RuleForEach(x => x.ActionAssets)
            .SetValidator(new ActionAssetDtoValidator());
    }
}

internal sealed class AbilityStatDtoValidator : AbstractValidator<AbilityStatDto>
{
    public AbilityStatDtoValidator()
    {
        RuleFor(x => x.Value).GreaterThan(0);
    }
}
