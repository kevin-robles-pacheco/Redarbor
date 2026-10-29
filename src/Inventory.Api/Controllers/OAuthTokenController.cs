using Inventory.Api.Contracts;
using Inventory.Application.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
namespace Inventory.Api.Controllers;
[ApiController]
[AllowAnonymous]
[EnableRateLimiting("oauth")]
public sealed class OAuthTokenController : ControllerBase
{
    private readonly ISender _sender;
    public OAuthTokenController(ISender sender)
    {
        _sender = sender;
    }
    [HttpPost("/oauth/token")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Create([FromForm] OAuthTokenRequest request, CancellationToken cancellationToken)
    {
        if (!string.Equals(request.GrantType, "client_credentials", StringComparison.Ordinal))
        {

            return BadRequest();
        }
        var token = await _sender.Send(new IssueAccessTokenCommand(request.ClientId ?? string.Empty, request.ClientSecret ?? string.Empty), cancellationToken);
        var expiresIn = (int)Math.Max(0, (token.ExpiresAtUtc - DateTime.UtcNow).TotalSeconds);

        return Ok(new AccessTokenResponse(token.Value, "Bearer", expiresIn));
    }
}
