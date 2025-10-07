using FluentValidation;

namespace Application.Game.Arenas.Update;

internal sealed class UpdateArenaCommandValidator : AbstractValidator<UpdateArenaCommand>
{
    public UpdateArenaCommandValidator()
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
