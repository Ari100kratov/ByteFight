using FluentValidation;

namespace Application.Game.Arenas.Create;

internal sealed class CreateArenaCommandValidator : AbstractValidator<CreateArenaCommand>
{
    public CreateArenaCommandValidator()
    {
        RuleFor(a => a.Name)
            .NotEmpty()
            .MaximumLength(64);

        RuleFor(a => a.BackgroundAsset)
            .MaximumLength(256);

        RuleFor(a => a.Description)
            .MaximumLength(256);

        RuleFor(a => a.GridWidth).GreaterThan(0);
        RuleFor(a => a.GridHeight).GreaterThan(0);
        RuleFor(a => a.GameModes).NotEmpty();
    }
}
