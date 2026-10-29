using Inventory.Application.Products;
using Inventory.Domain.Products;
namespace Inventory.Application.Tests.Fakes;
public sealed class FakeProductReadRepository : IProductReadRepository
{
    public Product? Stored { get; set; }
    public bool CategoryHasProducts { get; set; }
    public bool SkuExists { get; set; }
    public Task<Product?> GetByIdAsync(GetProductQuery query, CancellationToken cancellationToken)
    {

        return Task.FromResult(Stored);
    }
    public Task<IReadOnlyList<Product>> ListAsync(GetProductsQuery query, CancellationToken cancellationToken)
    {
        IReadOnlyList<Product> products = Stored is null ? Array.Empty<Product>() : new[] { Stored };

        return Task.FromResult(products);
    }
    public Task<bool> ExistsByCategoryAsync(CategoryProductLookup lookup, CancellationToken cancellationToken)
    {

        return Task.FromResult(CategoryHasProducts);
    }
    public Task<bool> ExistsBySkuAsync(SkuLookup lookup, CancellationToken cancellationToken)
    {

        return Task.FromResult(SkuExists);
    }
}
