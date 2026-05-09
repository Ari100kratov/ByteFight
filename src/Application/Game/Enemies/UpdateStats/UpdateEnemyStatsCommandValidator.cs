using Application.Contracts;
using FluentValidation;

namespace Application.Game.Enemies.UpdateStats;

internal sealed class UpdateEnemyStatsCommandValidator : AbstractValidator<UpdateEnemyStatsCommand>
{
    public UpdateEnemyStatsCommandValidator()
    {
        RuleFor(x => x.Stats)
            .NotNull();

        RuleForEach(x => x.Stats)
            .SetValidator(new StatDtoValidator());
    }
}
