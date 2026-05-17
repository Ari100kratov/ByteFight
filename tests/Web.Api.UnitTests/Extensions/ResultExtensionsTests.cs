using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;
using Shouldly;
using Web.Api.Extensions;
using Xunit;

namespace Web.Api.UnitTests.Extensions;

public sealed class ResultExtensionsTests
{
    [Fact]
    public void Match_ShouldUseSuccessBranch()
    {
        var result = Result.Success();

        string value = result.Match(
            onSuccess: () => "success",
            onFailure: _ => "failure");

        value.ShouldBe("success");
    }

    [Fact]
    public void Match_ShouldUseFailureBranch()
    {
        var result = Result.Failure(Error.NotFound("not-found", "Not found"));

        string value = result.Match(
            onSuccess: () => "success",
            onFailure: failure => failure.Error.Code);

        value.ShouldBe("not-found");
    }

    [Fact]
    public async Task ToCreated_ShouldWriteCreatedResponse()
    {
        var id = Guid.CreateVersion7();
        IResult result = Result.Success(id).ToCreated(value => $"/resources/{value}");

        DefaultHttpContext httpContext = CreateHttpContext();

        await result.ExecuteAsync(httpContext);

        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status201Created);
        httpContext.Response.Headers.Location.ToString().ShouldBe($"/resources/{id}");
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
