using Application.Abstractions.Behaviors;
using FluentValidation;
using SharedKernel;
using SharedKernel.Messaging;
using Shouldly;
using Xunit;

namespace Application.UnitTests.Abstractions.Behaviors;

public sealed class ValidationDecoratorTests
{
    [Fact]
    public async Task CommandHandler_ShouldReturnValidationErrorAndSkipInnerHandler()
    {
        var innerHandler = new TestCommandHandler(Result.Success("handled"));
        var validator = new TestCommandValidator();
        var handler = new ValidationDecorator.CommandHandler<TestCommand, string>(innerHandler, [validator]);

        Result<string> result = await handler.Handle(new TestCommand(""), CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeOfType<ValidationError>();
        result.Error.Type.ShouldBe(ErrorType.Validation);
        innerHandler.CallCount.ShouldBe(0);

        var validationError = (ValidationError)result.Error;
        validationError.Errors.Single().Code.ShouldBe("NotEmptyValidator");
    }

    [Fact]
    public async Task CommandHandler_ShouldCallInnerHandlerWhenCommandIsValid()
    {
        var innerHandler = new TestCommandHandler(Result.Success("handled"));
        var validator = new TestCommandValidator();
        var handler = new ValidationDecorator.CommandHandler<TestCommand, string>(innerHandler, [validator]);

        Result<string> result = await handler.Handle(new TestCommand("valid"), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("handled");
        innerHandler.CallCount.ShouldBe(1);
    }

    private sealed record TestCommand(string Value) : ICommand<string>;

    private sealed class TestCommandValidator : AbstractValidator<TestCommand>
    {
        public TestCommandValidator()
        {
            RuleFor(x => x.Value).NotEmpty();
        }
    }

    private sealed class TestCommandHandler(Result<string> result) : ICommandHandler<TestCommand, string>
    {
        public int CallCount { get; private set; }

        public Task<Result<string>> Handle(TestCommand command, CancellationToken cancellationToken)
        {
            CallCount++;
            return Task.FromResult(result);
        }
    }
}
