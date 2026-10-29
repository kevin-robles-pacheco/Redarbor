using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
namespace Inventory.Application;
public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
