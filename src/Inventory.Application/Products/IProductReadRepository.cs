using Inventory.Domain.Products;
namespace Inventory.Application.Products;
public interface IProductReadRepository
{
    Task<Product?> GetByIdAsync(GetProductQuery query, CancellationToken cancellationToken);
    Task<IReadOnlyList<Product>> ListAsync(GetProductsQuery query, CancellationToken cancellationToken);
    Task<bool> ExistsByCategoryAsync(CategoryProductLookup lookup, CancellationToken cancellationToken);
    Task<bool> ExistsBySkuAsync(SkuLookup lookup, CancellationToken cancellationToken);
}
