using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
namespace Inventory.Api.Swagger;
public static class ApiSwaggerRegistration
{
    public static IServiceCollection AddApiSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory API", Version = "v1" });
            AddOAuthSecurity(options);
        });

        return services;
    }
    private static void AddOAuthSecurity(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("oauth2", CreateScheme());
        options.AddSecurityRequirement(CreateRequirement());
    }
    private static OpenApiSecurityScheme CreateScheme()
    {

        return new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                ClientCredentials = new OpenApiOAuthFlow
                {
                    TokenUrl = new Uri("/oauth/token", UriKind.Relative),
                    Scopes = new Dictionary<string, string> { ["inventory"] = "Inventory API" }
                }
            }
        };
    }
    private static OpenApiSecurityRequirement CreateRequirement()
    {
        var scheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
        };

        return new OpenApiSecurityRequirement { [scheme] = new[] { "inventory" } };
    }
}
