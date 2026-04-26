using FluentValidation;

namespace Application.Auth.Users.UpdateProfile;

internal sealed class UpdateUserProfileCommandValidator
    : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(c => c.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(c => c.Email)
            .NotEmpty()
            .MaximumLength(256)
            .EmailAddress();
    }
}
