using Application.Contracts;
using FluentValidation;

namespace Application.Game.Enemies.UpdateAbilities;

internal sealed class UpdateEnemyAbilitiesCommandValidator : AbstractValidator<UpdateEnemyAbilitiesCommand>
{
    public UpdateEnemyAbilitiesCommandValidator()
    {
        RuleFor(x => x.Abilities)
            .NotNull();

        RuleForEach(x => x.Abilities)
            .SetValidator(new AbilityDtoValidator());
    }
}
