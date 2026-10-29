using Inventory.Api;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
namespace Inventory.Api.RateLimiting;
public static class ApiRateLimitRegistration
{
    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services, ApiSettings settings)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = ApiRateLimitRejection.WriteAsync;
            options.AddPolicy("api", httpContext => ApiRateLimitPartition.ForClient(httpContext, settings));
            options.AddPolicy("oauth", httpContext => ApiRateLimitPartition.ForAddress(httpContext, settings.OAuthPermitLimit, settings.WindowSeconds));
        });

        return services;
    }
}
