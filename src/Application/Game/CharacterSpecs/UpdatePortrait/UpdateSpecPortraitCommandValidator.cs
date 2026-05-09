using FluentValidation;

namespace Application.Game.CharacterSpecs.UpdatePortrait;

internal sealed class UpdateSpecPortraitCommandValidator : AbstractValidator<UpdateSpecPortraitCommand>
{
    public UpdateSpecPortraitCommandValidator()
    {
        RuleFor(x => x.PortraitUrl)
            .NotEmpty()
            .MaximumLength(256);
    }
}
