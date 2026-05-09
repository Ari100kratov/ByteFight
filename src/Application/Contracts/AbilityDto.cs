using Domain.Game.Abilities;
using Domain.Game.CharacterSpecAbilities;
using Domain.Game.Enemies;

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
