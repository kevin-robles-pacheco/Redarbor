using Inventory.Domain.Products;
namespace Inventory.Application.Products;
public sealed class ProductResponse
{
    public ProductResponse(Guid id, string name, string description, string sku, decimal price, Guid categoryId, int quantityOnHand, bool isActive, DateTime createdAtUtc)
    {
        Id = id;
        Name = name;
        Description = description;
        Sku = sku;
        Price = price;
        CategoryId = categoryId;
        QuantityOnHand = quantityOnHand;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
    }
    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public string Sku { get; }
    public decimal Price { get; }
    public Guid CategoryId { get; }
    public int QuantityOnHand { get; }
    public bool IsActive { get; }
    public DateTime CreatedAtUtc { get; }
    public static ProductResponse From(Product product)
    {

        return new ProductResponse(product.Id, product.Name, product.Description, product.Sku, product.Price, product.CategoryId, product.QuantityOnHand, product.IsActive, product.CreatedAtUtc);
    }
    public static IReadOnlyList<ProductResponse> From(IReadOnlyList<Product> products)
    {
        var responses = new List<ProductResponse>(products.Count);
        foreach (var product in products)
        {
            responses.Add(From(product));
        }

        return responses;
    }
}
