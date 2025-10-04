using FluentValidation;

namespace Application.Game.CharacterCodes.Update;

internal sealed class UpdateCodesCommandValidator : AbstractValidator<UpdateCodesCommand>
{
    public UpdateCodesCommandValidator()
    {
        RuleFor(c => c.CharacterId)
            .NotEmpty();

        RuleForEach(c => c.Created)
            .SetValidator(new CharacterCodeDtoValidator());

        RuleForEach(c => c.Updated)
            .SetValidator(new CharacterCodeDtoValidator());

        RuleForEach(c => c.DeletedIds)
            .NotEmpty();
    }

    private sealed class CharacterCodeDtoValidator : AbstractValidator<CharacterCodeDto>
    {
        public CharacterCodeDtoValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty();

            RuleFor(c => c.Name)
                .NotEmpty()
                .MaximumLength(32);
        }
    }
}
