using Inventory.Application.Authentication;
using Inventory.Application.Tests.Fakes;
namespace Inventory.Application.Tests;
public sealed class IssueAccessTokenHandlerTests
{
    [Fact]
    public async Task Handle_WhenCredentialsAreInvalid_ThrowsInvalidClientCredentialsException()
    {
        var issuer = new FakeAccessTokenIssuer();
        var handler = new IssueAccessTokenHandler(new FakeClientCredentialAuthenticator { IsValid = false }, issuer);
        var command = new IssueAccessTokenCommand("inventory-api", "wrong");
        await Assert.ThrowsAsync<InvalidClientCredentialsException>(() => handler.Handle(command, CancellationToken.None));
        Assert.Equal(0, issuer.IssueCalls);
    }
    [Fact]
    public async Task Handle_WhenCredentialsAreValid_ReturnsIssuedToken()
    {
        var issuer = new FakeAccessTokenIssuer();
        var handler = new IssueAccessTokenHandler(new FakeClientCredentialAuthenticator { IsValid = true }, issuer);
        var command = new IssueAccessTokenCommand("inventory-api", "local-dev-only");
        var token = await handler.Handle(command, CancellationToken.None);
        Assert.Equal("issued-token", token.Value);
        Assert.Equal(1, issuer.IssueCalls);
    }
}
