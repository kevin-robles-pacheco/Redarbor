namespace Inventory.Domain.Products;
public sealed class UpdateProduct
{
    public UpdateProduct(string name, string? description, string sku, decimal price, Guid categoryId)
    {
        Name = name;
        Description = description;
        Sku = sku;
        Price = price;
        CategoryId = categoryId;
    }
    public string Name { get; }
    public string? Description { get; }
    public string Sku { get; }
    public decimal Price { get; }
    public Guid CategoryId { get; }
}
