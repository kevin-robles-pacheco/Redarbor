using Inventory.Domain.Products;
namespace Inventory.Application.Products;
public sealed class ProductResponse
{
    private ProductResponse(Product product)
    {
        Id = product.Id;
        Name = product.Name;
        Description = product.Description;
        Sku = product.Sku;
        Price = product.Price;
        CategoryId = product.CategoryId;
        QuantityOnHand = product.QuantityOnHand;
        IsActive = product.IsActive;
        CreatedAtUtc = product.CreatedAtUtc;
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

        return new ProductResponse(product);
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
