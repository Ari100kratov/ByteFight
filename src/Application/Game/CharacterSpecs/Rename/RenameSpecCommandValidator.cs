using FluentValidation;

namespace Application.Game.CharacterSpecs.Rename;

internal sealed class RenameSpecCommandValidator : AbstractValidator<RenameSpecCommand>
{
    public RenameSpecCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(32);
    }
}
