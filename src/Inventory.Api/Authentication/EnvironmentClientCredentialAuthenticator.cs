using Inventory.Application.Authentication;
namespace Inventory.Api.Authentication;
public sealed class EnvironmentClientCredentialAuthenticator : IClientCredentialAuthenticator
{
    public bool CanAuthenticate(ClientCredentials credentials)
    {
        var clientId = Environment.GetEnvironmentVariable("OAUTH_CLIENT_ID");
        var clientSecret = Environment.GetEnvironmentVariable("OAUTH_CLIENT_SECRET");
        var clientIdMatches = string.Equals(credentials.ClientId, clientId, StringComparison.Ordinal);
        var clientSecretMatches = string.Equals(credentials.ClientSecret, clientSecret, StringComparison.Ordinal);

        return clientIdMatches && clientSecretMatches;
    }
}
