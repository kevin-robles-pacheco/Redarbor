using Inventory.Application.Categories;
using Inventory.Application.Movements;
using Inventory.Application.Persistence;
using Inventory.Application.Products;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace Inventory.Infrastructure;
public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        var connectionString = ReadConnectionString();
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        services.AddDbContext<InventoryDbContext>(options => options.UseNpgsql(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
        AddRepositories(services);

        return services;
    }
    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<ICategoryReadRepository, CategoryEfReadRepository>();
        services.AddScoped<ICategoryWriteRepository, CategoryDapperWriteRepository>();
        services.AddScoped<IProductReadRepository, ProductEfReadRepository>();
        services.AddScoped<IProductWriteRepository, ProductDapperWriteRepository>();
        services.AddScoped<IInventoryMovementReadRepository, InventoryMovementEfReadRepository>();
        services.AddScoped<IInventoryMovementWriter, InventoryMovementDapperWriter>();
        services.AddScoped<IDatabaseSchemaInitializer, PostgresSchemaInitializer>();
    }
    private static string ReadConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("POSTGRES_CONNECTION_STRING is required.");
        }

        return connectionString;
    }
}
