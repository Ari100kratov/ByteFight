using Domain.Game.CharacterSpecs;
using Domain.Game.Enemies;
using Domain.Game.Stats;
using FluentValidation;

namespace Application.Contracts;

public sealed record StatDto(StatType StatType, decimal Value);

internal static partial class Mapper
{
    public static StatDto ToDto(this EnemyStat entity) => new(entity.StatType, entity.Value);

    public static StatDto ToDto(this CharacterSpecStat entity) => new(entity.StatType, entity.Value);

    public static EnemyStat ToEnemyStat(this StatDto dto, Guid enemyId)
    {
        return new EnemyStat
        {
            EnemyId = enemyId,
            StatType = dto.StatType,
            Value = dto.Value
        };
    }

    public static CharacterSpecStat ToCharacterSpecStat(this StatDto dto, Guid characterSpecId)
    {
        return new CharacterSpecStat
        {
            CharacterSpecId = characterSpecId,
            StatType = dto.StatType,
            Value = dto.Value,
        };
    }
}

internal sealed class StatDtoValidator : AbstractValidator<StatDto>
{
    public StatDtoValidator()
    {
        RuleFor(x => x.Value).GreaterThan(0);
    }
}
