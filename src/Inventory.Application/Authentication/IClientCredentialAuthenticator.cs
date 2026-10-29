namespace Inventory.Application.Authentication;
public interface IClientCredentialAuthenticator
{
    bool CanAuthenticate(ClientCredentials credentials);
}
