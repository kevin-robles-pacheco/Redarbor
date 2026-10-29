using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
namespace Inventory.Api.RateLimiting;
public static class ApiRateLimitRejection
{
    public static async ValueTask WriteAsync(OnRejectedContext context, CancellationToken cancellationToken)
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter = Math.Ceiling(retryAfter.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }
        await Results.Problem(title: "Too many requests.", statusCode: StatusCodes.Status429TooManyRequests).ExecuteAsync(context.HttpContext);
    }
}
