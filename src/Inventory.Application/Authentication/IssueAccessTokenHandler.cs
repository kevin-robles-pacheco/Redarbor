using MediatR;
namespace Inventory.Application.Authentication;
public sealed class IssueAccessTokenHandler : IRequestHandler<IssueAccessTokenCommand, AccessToken>
{
    private readonly IClientCredentialAuthenticator _authenticator;
    private readonly IAccessTokenIssuer _issuer;
    public IssueAccessTokenHandler(IClientCredentialAuthenticator authenticator, IAccessTokenIssuer issuer)
    {
        _authenticator = authenticator;
        _issuer = issuer;
    }
    public Task<AccessToken> Handle(IssueAccessTokenCommand command, CancellationToken cancellationToken)
    {
        var credentials = new ClientCredentials(command.ClientId, command.ClientSecret);
        if (!_authenticator.CanAuthenticate(credentials))
        {
            throw new InvalidClientCredentialsException();
        }

        return Task.FromResult(_issuer.Issue(credentials));
    }
}
