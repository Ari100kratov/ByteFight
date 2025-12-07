using Application.Contracts;
using FluentValidation;

namespace Application.Game.Enemies.Update;

internal sealed class UpdateEnemyCommandValidator : AbstractValidator<UpdateEnemyCommand>
{
    public UpdateEnemyCommandValidator()
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
