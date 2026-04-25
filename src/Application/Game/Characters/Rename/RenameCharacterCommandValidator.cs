using FluentValidation;

namespace Application.Game.Characters.Rename;

internal sealed class RenameCharacterCommandValidator : AbstractValidator<RenameCharacterCommand>
{
    public RenameCharacterCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(32);
    }
}
