using Domain.Game.Arenas.ArenaEnemies;
using FluentValidation;

namespace Application.Game.Arenas.Enemies.Dtos;

public sealed record PositionDto(int X, int Y);

internal static partial class Mapper
{
    public static PositionDto ToDto(this Position valueObject) => new(valueObject.X, valueObject.Y);

    public static Position ToValueObject(this PositionDto dto) => new(dto.X, dto.Y);
}

internal sealed class PositionDtoValidator : AbstractValidator<PositionDto>
{
    public PositionDtoValidator()
    {
        RuleFor(x => x.X)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Y)
            .GreaterThanOrEqualTo(0);
    }
}
