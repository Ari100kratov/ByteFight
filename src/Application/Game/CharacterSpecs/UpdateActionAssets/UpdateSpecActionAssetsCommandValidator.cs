using Application.Contracts;
using FluentValidation;

namespace Application.Game.CharacterSpecs.UpdateActionAssets;

internal sealed class UpdateSpecActionAssetsCommandValidator : AbstractValidator<UpdateSpecActionAssetsCommand>
{
    public UpdateSpecActionAssetsCommandValidator()
    {
        RuleFor(x => x.ActionAssets)
            .NotNull();

        RuleForEach(x => x.ActionAssets)
            .SetValidator(new ActionAssetDtoValidator());
    }
}
