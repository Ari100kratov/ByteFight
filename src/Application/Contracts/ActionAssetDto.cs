using Domain.Game.Actions;
using Domain.Game.CharacterClasses;
using Domain.Game.Enemies;
using FluentValidation;

namespace Application.Contracts;

public sealed record ActionAssetDto(ActionType ActionType, int Variant, SpriteAnimationDto SpriteAnimation);

internal static partial class Mapper
{
    public static ActionAssetDto ToDto(this EnemyActionAsset entity)
    {
        return new ActionAssetDto(
            entity.ActionType,
            entity.Variant,
            entity.Animation.ToDto()
        );
    }

    public static ActionAssetDto ToDto(this CharacterClassActionAsset entity)
    {
        return new ActionAssetDto(
            entity.ActionType,
            entity.Variant,
            entity.Animation.ToDto()
        );
    }

    public static EnemyActionAsset ToEnemyActionAsset(this ActionAssetDto dto, Guid enemyId)
    {
        return new EnemyActionAsset
        {
            EnemyId = enemyId,
            ActionType = dto.ActionType,
            Variant = dto.Variant,
            Animation = dto.SpriteAnimation.ToValueObject()
        };
    }

    public static CharacterClassActionAsset ToCharacterClassActionAsset(this ActionAssetDto dto, Guid characterClassId)
    {
        return new CharacterClassActionAsset
        {
            CharacterClassId = characterClassId,
            ActionType = dto.ActionType,
            Variant = dto.Variant,
            Animation = dto.SpriteAnimation.ToValueObject()
        };
    }
}

internal sealed class ActionAssetDtoValidator : AbstractValidator<ActionAssetDto>
{
    public ActionAssetDtoValidator()
    {
        RuleFor(x => x.Variant).GreaterThanOrEqualTo(0);

        RuleFor(x => x.SpriteAnimation).NotNull();
        RuleFor(x => x.SpriteAnimation)
            .SetValidator(new SpriteAnimationDtoValidator());
    }
}
