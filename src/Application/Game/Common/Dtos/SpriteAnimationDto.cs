using Domain.ValueObjects;
using FluentValidation;

namespace Application.Game.Common.Dtos;

public sealed record SpriteAnimationDto(
    Uri Url,
    int FrameCount,
    float AnimationSpeed,
    ScaleDto Scale
);

public sealed record ScaleDto(float X, float Y);

internal static partial class Mapper
{
    public static SpriteAnimationDto ToDto(this SpriteAnimation valueObject)
    {
        return new SpriteAnimationDto(
            new Uri(valueObject.Url, UriKind.Relative),
            valueObject.FrameCount,
            valueObject.AnimationSpeed,
            new ScaleDto(valueObject.Scale.X, valueObject.Scale.Y)
        );
    }

    public static SpriteAnimation ToValueObject(this SpriteAnimationDto dto)
    {
        return new SpriteAnimation(
            dto.Url,
            dto.FrameCount,
            dto.AnimationSpeed,
            dto.Scale.X,
            dto.Scale.Y
        );
    }
}

internal sealed class SpriteAnimationDtoValidator : AbstractValidator<SpriteAnimationDto>
{
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

        RuleFor(x => x.Scale).NotNull();
        RuleFor(x => x.Scale)
            .SetValidator(new ScaleDtoValidator());
    }
}

internal sealed class ScaleDtoValidator : AbstractValidator<ScaleDto>
{
    private const float Epsilon = 0.0001f;

    public ScaleDtoValidator()
    {
        RuleFor(x => x.X)
            .Must(x => Math.Abs(x) > Epsilon)
            .WithMessage($"{nameof(SpriteAnimationDto.Scale.X)} не может быть равен 0.");

        RuleFor(x => x.Y)
            .Must(y => Math.Abs(y) > Epsilon)
            .WithMessage($"{nameof(SpriteAnimationDto.Scale.Y)} не может быть равен 0.");
    }
}
