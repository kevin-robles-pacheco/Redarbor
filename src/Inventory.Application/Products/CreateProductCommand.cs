using MediatR;
namespace Inventory.Application.Products;
public sealed class CreateProductCommand : IRequest<ProductResponse>
{
    public CreateProductCommand(string name, string? description, string sku, decimal price, Guid categoryId, DateTime createdAtUtc)
    {
        Name = name;
        Description = description;
        Sku = sku;
        Price = price;
        CategoryId = categoryId;
        CreatedAtUtc = createdAtUtc;
    }
    public string Name { get; }
    public string? Description { get; }
    public string Sku { get; }
    public decimal Price { get; }
    public Guid CategoryId { get; }
    public DateTime CreatedAtUtc { get; }
}
