using FluentValidation;

namespace Application.Game.Enemies.Update;

internal sealed class UpdateEnemyCommandValidator : AbstractValidator<UpdateEnemyCommand>
{
    public UpdateEnemyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(32);

        RuleFor(x => x.Description)
            .MaximumLength(512);

        RuleForEach(x => x.Stats)
            .SetValidator(new EnemyStatDtoValidator());

        RuleForEach(x => x.Assets)
            .SetValidator(new EnemyAssetDtoValidator());
    }

    private sealed class EnemyStatDtoValidator : AbstractValidator<EnemyStatDto>
    {
        public EnemyStatDtoValidator()
        {
            RuleFor(x => x.Value).GreaterThan(0);
        }
    }

    private sealed class EnemyAssetDtoValidator : AbstractValidator<EnemyAssetDto>
    {
        public EnemyAssetDtoValidator()
        {
            RuleFor(x => x.Url.ToString())
                .NotEmpty()
                .MaximumLength(256);
        }
    }
}
