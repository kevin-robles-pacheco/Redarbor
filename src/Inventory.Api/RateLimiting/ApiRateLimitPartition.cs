using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
namespace Inventory.Api.RateLimiting;
public static class ApiRateLimitPartition
{
    public static RateLimitPartition<string> ForClient(HttpContext httpContext, ApiSettings settings)
    {
        var key = httpContext.User.FindFirst("client_id")?.Value ?? RemoteAddress(httpContext);

        return FixedWindow(key, settings.PermitLimit, settings.WindowSeconds);
    }
    public static RateLimitPartition<string> ForAddress(HttpContext httpContext, int permitLimit, int windowSeconds)
    {

        return FixedWindow(RemoteAddress(httpContext), permitLimit, windowSeconds);
    }
    private static string RemoteAddress(HttpContext httpContext)
    {

        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
    private static RateLimitPartition<string> FixedWindow(string key, int permitLimit, int windowSeconds)
    {

        return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = permitLimit,
            Window = TimeSpan.FromSeconds(windowSeconds),
            QueueLimit = 0
        });
    }
}
