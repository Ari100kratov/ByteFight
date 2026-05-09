using Application.Contracts;
using FluentValidation;

namespace Application.Game.Enemies.UpdateActionAssets;

internal sealed class UpdateEnemyActionAssetsCommandValidator : AbstractValidator<UpdateEnemyActionAssetsCommand>
{
    public UpdateEnemyActionAssetsCommandValidator()
    {
        RuleFor(x => x.ActionAssets)
            .NotNull();

        RuleForEach(x => x.ActionAssets)
            .SetValidator(new ActionAssetDtoValidator());
    }
}
