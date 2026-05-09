using FluentValidation;

namespace Application.Game.Enemies.Rename;

internal sealed class RenameEnemyCommandValidator : AbstractValidator<RenameEnemyCommand>
{
    public RenameEnemyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(32);
    }
}
