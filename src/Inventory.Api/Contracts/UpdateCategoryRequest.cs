namespace Inventory.Api.Contracts;
public sealed class UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
