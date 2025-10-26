using Application.Game.Common.Dtos;
using FluentValidation;

namespace Application.Game.CharacterClasses.Update;

internal sealed class UpdateClassCommandValidator : AbstractValidator<UpdateClassCommand>
{
    public UpdateClassCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(32);

        RuleFor(x => x.Description)
            .MaximumLength(512);

        RuleForEach(x => x.Stats)
            .SetValidator(new StatDtoValidator());

        RuleForEach(x => x.ActionAssets)
            .SetValidator(new ActionAssetDtoValidator());
    }
}
