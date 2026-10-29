using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Inventory.Application.Authentication;
using Microsoft.IdentityModel.Tokens;
namespace Inventory.Api.Authentication;
public sealed class JwtAccessTokenIssuer : IAccessTokenIssuer
{
    private readonly ApiSettings _settings;
    public JwtAccessTokenIssuer(ApiSettings settings)
    {
        _settings = settings;
    }
    public AccessToken Issue(ClientCredentials credentials)
    {
        var expiresAtUtc = DateTime.UtcNow.AddSeconds(_settings.JwtExpiresInSeconds);
        var handler = new JwtSecurityTokenHandler();
        var value = handler.WriteToken(handler.CreateToken(CreateDescriptor(credentials, expiresAtUtc)));

        return new AccessToken(value, expiresAtUtc);
    }
    private SecurityTokenDescriptor CreateDescriptor(ClientCredentials credentials, DateTime expiresAtUtc)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtSigningKey));

        return new SecurityTokenDescriptor
        {
            Issuer = _settings.JwtIssuer,
            Audience = _settings.JwtAudience,
            Expires = expiresAtUtc,
            Subject = new ClaimsIdentity(new[] { new Claim("client_id", credentials.ClientId) }),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };
    }
}
