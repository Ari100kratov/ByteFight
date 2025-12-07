using Application.Contracts;
using FluentValidation;

namespace Application.Game.Enemies.Create;

internal sealed class CreateEnemyCommandValidator : AbstractValidator<CreateEnemyCommand>
{
    public CreateEnemyCommandValidator()
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
