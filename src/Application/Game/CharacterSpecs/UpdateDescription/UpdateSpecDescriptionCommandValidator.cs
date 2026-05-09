using FluentValidation;

namespace Application.Game.CharacterSpecs.UpdateDescription;

internal sealed class UpdateSpecDescriptionCommandValidator : AbstractValidator<UpdateSpecDescriptionCommand>
{
    public UpdateSpecDescriptionCommandValidator()
    {
        RuleFor(x => x.Description)
            .MaximumLength(512);
    }
}
