using FluentValidation;

namespace Application.Game.Enemies.UpdateDescription;

internal sealed class UpdateEnemyDescriptionCommandValidator : AbstractValidator<UpdateEnemyDescriptionCommand>
{
    public UpdateEnemyDescriptionCommandValidator()
    {
        RuleFor(x => x.Description)
            .MaximumLength(512);
    }
}
