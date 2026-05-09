using Application.Contracts;
using FluentValidation;

namespace Application.Game.CharacterSpecs.UpdateStats;

internal sealed class UpdateSpecStatsCommandValidator : AbstractValidator<UpdateSpecStatsCommand>
{
    public UpdateSpecStatsCommandValidator()
    {
        RuleFor(x => x.Stats)
            .NotNull();

        RuleForEach(x => x.Stats)
            .SetValidator(new StatDtoValidator());
    }
}
