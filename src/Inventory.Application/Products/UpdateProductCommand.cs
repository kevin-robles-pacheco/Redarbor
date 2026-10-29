using MediatR;
namespace Inventory.Application.Products;
public sealed class UpdateProductCommand : IRequest<ProductResponse>
{
    public UpdateProductCommand(Guid productId, string name, string? description, string sku, decimal price, Guid categoryId)
    {
        ProductId = productId;
        Name = name;
        Description = description;
        Sku = sku;
        Price = price;
        CategoryId = categoryId;
    }
    public Guid ProductId { get; }
    public string Name { get; }
    public string? Description { get; }
    public string Sku { get; }
    public decimal Price { get; }
    public Guid CategoryId { get; }
}
