using Domain.ValueObjects;
using FluentValidation;

namespace Application.Game.Common.Dtos;

public sealed record SpriteAnimationDto(
    Uri Url,
    int FrameCount,
    float AnimationSpeed,
    float ScaleX,
    float ScaleY
);

internal static partial class Mapper
{
    public static SpriteAnimationDto ToDto(this SpriteAnimation valueObject)
    {
        return new SpriteAnimationDto(
            new Uri(valueObject.Url, UriKind.Relative),
            valueObject.FrameCount,
            valueObject.AnimationSpeed,
            valueObject.ScaleX,
            valueObject.ScaleY
        );
    }

    public static SpriteAnimation ToValueObject(this SpriteAnimationDto dto)
    {
        return new SpriteAnimation(
            dto.Url,
            dto.FrameCount,
            dto.AnimationSpeed,
            dto.ScaleX,
            dto.ScaleY
        );
    }
}

internal sealed class SpriteAnimationDtoValidator : AbstractValidator<SpriteAnimationDto>
{
    private const float Epsilon = 0.0001f;

    public SpriteAnimationDtoValidator()
    {
        RuleFor(x => x.Url)
            .NotNull()
            .Must(uri => !string.IsNullOrWhiteSpace(uri.ToString()))
            .WithMessage($"{nameof(SpriteAnimationDto.Url)} не может быть пустым.")
            .Must(uri => uri.ToString().Length <= 256)
            .WithMessage($"{nameof(SpriteAnimationDto.Url)} не может превышать 256 символов.");

        RuleFor(x => x.FrameCount)
            .GreaterThan(0)
            .WithMessage($"{nameof(SpriteAnimationDto.FrameCount)} должен быть больше 0.");

        RuleFor(x => x.AnimationSpeed)
            .GreaterThan(0)
            .WithMessage($"{nameof(SpriteAnimationDto.AnimationSpeed)} должен быть больше 0.");

        RuleFor(x => x.ScaleX)
            .Must(x => Math.Abs(x) > Epsilon)
            .WithMessage($"{nameof(SpriteAnimationDto.ScaleX)} не может быть равен 0.");

        RuleFor(x => x.ScaleY)
            .Must(y => Math.Abs(y) > Epsilon)
            .WithMessage($"{nameof(SpriteAnimationDto.ScaleY)} не может быть равен 0.");
    }
}
