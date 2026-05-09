using FluentValidation;

namespace Application.Game.CharacterSpecs.UpdatePortrait;

internal sealed class UpdateSpecPortraitCommandValidator : AbstractValidator<UpdateSpecPortraitCommand>
{
    public UpdateSpecPortraitCommandValidator()
    {
        RuleFor(x => x.PortraitUrl)
            .NotNull()
            .Must(uri => !string.IsNullOrWhiteSpace(uri.ToString()))
            .WithMessage($"{nameof(UpdateSpecPortraitCommand.PortraitUrl)} не может быть пустым.")
            .Must(uri => uri.ToString().Length <= 256)
            .WithMessage($"{nameof(UpdateSpecPortraitCommand.PortraitUrl)} не может превышать 256 символов.");
    }
}
