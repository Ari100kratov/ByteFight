using Application.Contracts;
using FluentValidation;

namespace Application.Game.CharacterSpecs.Update;

internal sealed class UpdateSpecCommandValidator : AbstractValidator<UpdateSpecCommand>
{
    public UpdateSpecCommandValidator()
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
