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

        When(a => a.StartPosition != null, () =>
        {
            RuleFor(a => a.StartPosition!)
                .Must((command, pos) =>
                    pos.X >= 0 && pos.X < command.GridWidth)
                .WithMessage("StartPosition.X выходит за границы арены");

            RuleFor(a => a.StartPosition!)
                .Must((command, pos) =>
                    pos.Y >= 0 && pos.Y < command.GridHeight)
                .WithMessage("StartPosition.Y выходит за границы арены");
        });

        When(a => a.BlockedPositions != null && a.BlockedPositions.Any(), () =>
        {
            RuleForEach(a => a.BlockedPositions!)
                .Must((command, pos) =>
                    pos.X >= 0 && pos.X < command.GridWidth &&
                    pos.Y >= 0 && pos.Y < command.GridHeight)
                .WithMessage((_, position) =>
                    $"BlockedPosition ({position.X}, {position.Y}) выходит за границы арены");
        });
    }
}
