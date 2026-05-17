using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;
using Shouldly;
using Web.Api.Infrastructure;
using Xunit;

namespace Web.Api.UnitTests.Infrastructure;

public sealed class CustomResultsTests
{
    [Theory]
    [InlineData(ErrorType.Problem, StatusCodes.Status400BadRequest)]
    [InlineData(ErrorType.Validation, StatusCodes.Status400BadRequest)]
    [InlineData(ErrorType.NotFound, StatusCodes.Status404NotFound)]
    [InlineData(ErrorType.Conflict, StatusCodes.Status409Conflict)]
    [InlineData(ErrorType.Failure, StatusCodes.Status500InternalServerError)]
    public async Task Problem_ShouldMapErrorTypeToHttpStatus(ErrorType errorType, int expectedStatusCode)
    {
        var result = Result.Failure(new Error("test.error", "Test error", errorType));
        IResult httpResult = CustomResults.Problem(result);

        DefaultHttpContext httpContext = CreateHttpContext();

        await httpResult.ExecuteAsync(httpContext);

        httpContext.Response.StatusCode.ShouldBe(expectedStatusCode);
    }

    [Fact]
    public void Problem_ShouldRejectSuccessfulResult()
    {
        Should.Throw<InvalidOperationException>(() => CustomResults.Problem(Result.Success()));
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        ServiceProvider services = new ServiceCollection()
            .AddLogging()
            .AddProblemDetails()
            .BuildServiceProvider();

        return new DefaultHttpContext
        {
            RequestServices = services,
            Response =
            {
                Body = new MemoryStream()
            }
        };
    }
}
