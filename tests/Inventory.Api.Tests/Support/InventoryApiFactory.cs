using Inventory.Application.Categories;
using Inventory.Application.Movements;
using Inventory.Application.Persistence;
using Inventory.Application.Products;
using Inventory.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace Inventory.Api.Tests.Support;
public sealed class InventoryApiFactory : WebApplicationFactory<Program>
{
    private readonly InMemoryInventoryStore _store = new();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        SetEnvironment();
        builder.ConfigureTestServices(services =>
        {
            RemoveInfrastructure(services);
            services.AddSingleton(_store);
            services.AddSingleton<ICategoryReadRepository>(_store);
            services.AddSingleton<ICategoryWriteRepository>(_store);
            services.AddSingleton<IProductReadRepository>(_store);
            services.AddSingleton<IProductWriteRepository>(_store);
            services.AddSingleton<IInventoryMovementReadRepository>(_store);
            services.AddSingleton<IInventoryMovementWriter>(_store);
            services.AddSingleton<IDatabaseSchemaInitializer>(_store);
            services.AddSingleton<IDbConnectionFactory>(_store);
        });
    }
    private static void SetEnvironment()
    {
        Environment.SetEnvironmentVariable("POSTGRES_CONNECTION_STRING", "Host=localhost;Database=inventory;Username=inventory;Password=local-dev-only");
        Environment.SetEnvironmentVariable("OAUTH_CLIENT_ID", "inventory-api");
        Environment.SetEnvironmentVariable("OAUTH_CLIENT_SECRET", "local-dev-only");
        Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", "local-dev-only-signing-key-32-bytes");
        Environment.SetEnvironmentVariable("JWT_ISSUER", "inventory-api");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "inventory-api");
        Environment.SetEnvironmentVariable("JWT_EXPIRES_IN_SECONDS", "3600");
        Environment.SetEnvironmentVariable("RATE_LIMIT_PERMIT_LIMIT", "1000");
        Environment.SetEnvironmentVariable("RATE_LIMIT_WINDOW_SECONDS", "60");
        Environment.SetEnvironmentVariable("OAUTH_RATE_LIMIT_PERMIT_LIMIT", "1000");
    }
    private static void RemoveInfrastructure(IServiceCollection services)
    {
        var removable = services.Where(IsInfrastructureRegistration).ToList();
        foreach (var descriptor in removable)
        {
            services.Remove(descriptor);
        }
    }
    private static bool IsInfrastructureRegistration(ServiceDescriptor descriptor)
    {
        var serviceType = descriptor.ServiceType;
        if (serviceType == typeof(ICategoryReadRepository) || serviceType == typeof(ICategoryWriteRepository) || serviceType == typeof(IProductReadRepository) || serviceType == typeof(IProductWriteRepository) || serviceType == typeof(IInventoryMovementReadRepository) || serviceType == typeof(IInventoryMovementWriter) || serviceType == typeof(IDatabaseSchemaInitializer) || serviceType == typeof(IDbConnectionFactory))
        {

            return true;
        }
        if (serviceType == typeof(DbContextOptions<InventoryDbContext>) || serviceType == typeof(InventoryDbContext))
        {

            return true;
        }
        if (descriptor.ImplementationType is Type implementation && implementation.Namespace is not null && implementation.Namespace.StartsWith("Inventory.Infrastructure", StringComparison.Ordinal))
        {

            return true;
        }

        return false;
    }
}
