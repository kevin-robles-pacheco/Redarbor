using Inventory.Application.Authentication;
namespace Inventory.Application.Tests.Fakes;
public sealed class FakeClientCredentialAuthenticator : IClientCredentialAuthenticator
{
    public bool IsValid { get; set; }
    public bool CanAuthenticate(ClientCredentials credentials)
    {

        return IsValid;
    }
}
