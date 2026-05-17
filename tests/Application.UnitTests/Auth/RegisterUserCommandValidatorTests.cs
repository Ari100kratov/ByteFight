using Application.Auth.Users.Register;
using Shouldly;
using Xunit;

namespace Application.UnitTests.Auth;

public sealed class RegisterUserCommandValidatorTests
{
    [Fact]
    public void Validate_ShouldRejectInvalidRegistrationData()
    {
        var validator = new RegisterUserCommandValidator();
        var command = new RegisterUserCommand(
            Email: "not-email",
            FirstName: "",
            LastName: "",
            Password: "short");

        FluentValidation.Results.ValidationResult result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.Select(x => x.PropertyName).ShouldContain(nameof(RegisterUserCommand.Email));
        result.Errors.Select(x => x.PropertyName).ShouldContain(nameof(RegisterUserCommand.FirstName));
        result.Errors.Select(x => x.PropertyName).ShouldContain(nameof(RegisterUserCommand.LastName));
        result.Errors.Select(x => x.PropertyName).ShouldContain(nameof(RegisterUserCommand.Password));
    }

    [Fact]
    public void Validate_ShouldAcceptValidRegistrationData()
    {
        var validator = new RegisterUserCommandValidator();
        var command = new RegisterUserCommand(
            Email: "ivan@example.test",
            FirstName: "Иван",
            LastName: "Петров",
            Password: "safe-password");

        FluentValidation.Results.ValidationResult result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }
}
