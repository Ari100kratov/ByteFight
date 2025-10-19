using FluentValidation;

namespace Application.Game.Arenas.Enemies.Add;

internal sealed class AddEnemyToArenaCommandValidator : AbstractValidator<AddEnemyToArenaCommand>
{
    public AddEnemyToArenaCommandValidator()
    {
        RuleFor(x => x.ArenaId)
            .NotEmpty();

        RuleFor(x => x.EnemyId)
            .NotEmpty();

        RuleFor(x => x.Position)
            .NotNull();

        RuleFor(x => x.Position.X)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Координата X не может быть отрицательной.");

        RuleFor(x => x.Position.Y)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Координата Y не может быть отрицательной.");
    }
}
