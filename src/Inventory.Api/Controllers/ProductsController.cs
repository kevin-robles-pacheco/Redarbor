using Inventory.Api.Contracts;
using Inventory.Application.Products;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
namespace Inventory.Api.Controllers;
[ApiController]
[Authorize]
[EnableRateLimiting("api")]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly ISender _sender;
    public ProductsController(ISender sender)
    {
        _sender = sender;
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(request.Name, request.Description, request.Sku, request.Price, request.CategoryId, DateTime.UtcNow);
        var response = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { productId = response.Id }, response);
    }
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] Guid? categoryId, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new GetProductsQuery(categoryId), cancellationToken);

        return Ok(response);
    }
    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetById(Guid productId, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new GetProductQuery(productId), cancellationToken);

        return Ok(response);
    }
    [HttpPut("{productId:guid}")]
    public async Task<IActionResult> Update(Guid productId, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(productId, request.Name, request.Description, request.Sku, request.Price, request.CategoryId);
        var response = await _sender.Send(command, cancellationToken);

        return Ok(response);
    }
    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> Delete(Guid productId, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteProductCommand(productId), cancellationToken);

        return NoContent();
    }
}
