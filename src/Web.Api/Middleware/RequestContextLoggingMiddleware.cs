using System.Diagnostics;
using System.Security.Claims;
using Microsoft.Extensions.Primitives;

namespace Web.Api.Middleware;

public class RequestContextLoggingMiddleware(RequestDelegate next, ILogger<RequestContextLoggingMiddleware> logger)
{
    private const string CorrelationIdHeaderName = "CorrelationId";

    public Task Invoke(HttpContext context)
    {
        var data = new Dictionary<string, object>
        {
            ["CorrelationId"] = GetCorrelationId(context)
        };

        string? userId = GetUserId(context);
        if (userId is not null)
        {
            Activity.Current?.SetTag("user.id", userId);

            data["UserId"] = userId;
        }

        using (logger.BeginScope(data))
        {
            return next.Invoke(context);
        }
    }

    private static string? GetUserId(HttpContext context)
    {
        return context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    private static string GetCorrelationId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(
            CorrelationIdHeaderName,
            out StringValues correlationId);

        return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
    }
}
