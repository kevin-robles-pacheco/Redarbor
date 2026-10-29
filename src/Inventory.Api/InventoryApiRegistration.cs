using Inventory.Api.Authentication;
using Inventory.Api.Errors;
using Inventory.Api.RateLimiting;
using Inventory.Api.Swagger;
using Inventory.Application.Persistence;
using Microsoft.Extensions.DependencyInjection;
namespace Inventory.Api;
public static class InventoryApiRegistration
{
    public static IServiceCollection AddInventoryApi(this IServiceCollection services)
    {
        var settings = ApiSettings.FromEnvironment();
        services.AddSingleton(settings);
        services.AddControllers();
        services.AddExceptionHandler<ApiExceptionHandler>();
        services.AddProblemDetails();
        services.AddApiAuthentication(settings);
        services.AddApiRateLimiting(settings);
        services.AddApiSwagger();

        return services;
    }
    public static WebApplication UseInventoryApi(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRateLimiter();
        app.MapControllers();
        app.MapGet("/health", () => Results.Ok());

        return app;
    }
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseSchemaInitializer>();
        await initializer.InitializeAsync(CancellationToken.None);
    }
}
