using Inventory.Application.Authentication;
namespace Inventory.Application.Tests.Fakes;
public sealed class FakeAccessTokenIssuer : IAccessTokenIssuer
{
    public int IssueCalls { get; private set; }
    public AccessToken Issue(ClientCredentials credentials)
    {
        IssueCalls++;

        return new AccessToken("issued-token", new DateTime(2026, 10, 28, 13, 0, 0, DateTimeKind.Utc));
    }
}
