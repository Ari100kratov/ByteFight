using Application.Game.Arenas.Enemies.Dtos;
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
            .NotNull()
            .SetValidator(new PositionDtoValidator());
    }
}
