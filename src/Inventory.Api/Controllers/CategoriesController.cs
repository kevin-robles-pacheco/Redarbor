using Inventory.Api.Contracts;
using Inventory.Application.Categories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
namespace Inventory.Api.Controllers;
[ApiController]
[Authorize]
[EnableRateLimiting("api")]
[Route("api/categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly ISender _sender;
    public CategoriesController(ISender sender)
    {
        _sender = sender;
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(request.Name, request.Description, DateTime.UtcNow);
        var response = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { categoryId = response.Id }, response);
    }
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new GetCategoriesQuery(), cancellationToken);

        return Ok(response);
    }
    [HttpGet("{categoryId:guid}")]
    public async Task<IActionResult> GetById(Guid categoryId, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new GetCategoryQuery(categoryId), cancellationToken);

        return Ok(response);
    }
    [HttpPut("{categoryId:guid}")]
    public async Task<IActionResult> Update(Guid categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand(categoryId, request.Name, request.Description);
        var response = await _sender.Send(command, cancellationToken);

        return Ok(response);
    }
    [HttpDelete("{categoryId:guid}")]
    public async Task<IActionResult> Delete(Guid categoryId, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteCategoryCommand(categoryId), cancellationToken);

        return NoContent();
    }
}
