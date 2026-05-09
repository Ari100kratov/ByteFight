using Application.Contracts;
using FluentValidation;

namespace Application.Game.CharacterSpecs.UpdateAbilities;

internal sealed class UpdateSpecAbilitiesCommandValidator : AbstractValidator<UpdateSpecAbilitiesCommand>
{
    public UpdateSpecAbilitiesCommandValidator()
    {
        RuleFor(x => x.Abilities)
            .NotNull();

        RuleForEach(x => x.Abilities)
            .SetValidator(new AbilityDtoValidator());
    }
}
