namespace Inventory.Application.Authentication;
public interface IAccessTokenIssuer
{
    AccessToken Issue(ClientCredentials credentials);
}
