using MediatR;
namespace Inventory.Application.Authentication;
public sealed class IssueAccessTokenCommand : IRequest<AccessToken>
{
    public IssueAccessTokenCommand(string clientId, string clientSecret)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
    }
    public string ClientId { get; }
    public string ClientSecret { get; }
}
