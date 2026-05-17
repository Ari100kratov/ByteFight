using Shouldly;
using Xunit;

namespace SharedKernel.UnitTests;

public sealed class ResultTests
{
    [Fact]
    public void Success_ShouldExposeValue()
    {
        var result = Result.Success("ok");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("ok");
        result.Error.ShouldBe(Error.None);
    }

    [Fact]
    public void Failure_ShouldHideValueAndExposeError()
    {
        var error = Error.Problem("test.problem", "Test failure");
        var result = Result.Failure<string>(error);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
        Should.Throw<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Constructor_ShouldRejectInvalidSuccessErrorCombination()
    {
        Should.Throw<ArgumentException>(() => new Result(isSuccess: true, Error.Problem("bad", "Bad")));
        Should.Throw<ArgumentException>(() => new Result(isSuccess: false, Error.None));
    }

    [Fact]
    public void ImplicitConversion_ShouldTreatNullAsFailure()
    {
        string? value = null;
        Result<string> result = value;

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(Error.NullValue);
    }
}
