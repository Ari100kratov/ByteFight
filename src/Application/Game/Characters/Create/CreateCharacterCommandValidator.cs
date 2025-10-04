using FluentValidation;

namespace Application.Game.Characters.Create;

internal sealed class CreateCharacterCommandValidator : AbstractValidator<CreateCharacterCommand>
{
    public CreateCharacterCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(32);
    }
}
