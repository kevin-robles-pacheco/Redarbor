using Inventory.Api.Contracts;
using Inventory.Application.Movements;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
namespace Inventory.Api.Controllers;
[ApiController]
[Authorize]
[EnableRateLimiting("api")]
[Route("api/inventory-movements")]
public sealed class InventoryMovementsController : ControllerBase
{
    private readonly ISender _sender;
    public InventoryMovementsController(ISender sender)
    {
        _sender = sender;
    }
    [HttpPost]
    public async Task<IActionResult> Create(RegisterInventoryMovementRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterInventoryMovementCommand(request.ProductId, MovementTypeParser.Parse(request.Type), request.Quantity, DateTime.UtcNow, request.Notes);
        var response = await _sender.Send(command, cancellationToken);

        return Ok(response);
    }
}
