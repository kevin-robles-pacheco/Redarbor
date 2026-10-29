using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Inventory.Application.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
namespace Inventory.Api.Authentication;
public static class ApiAuthenticationRegistration
{
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, ApiSettings settings)
    {
        services.AddSingleton<IClientCredentialAuthenticator, EnvironmentClientCredentialAuthenticator>();
        services.AddSingleton<IAccessTokenIssuer, JwtAccessTokenIssuer>();
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => ConfigureBearer(options, settings));
        services.AddAuthorization();

        return services;
    }
    private static void ConfigureBearer(JwtBearerOptions options, ApiSettings settings)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = settings.JwtIssuer,
            ValidAudience = settings.JwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtSigningKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    }
}
