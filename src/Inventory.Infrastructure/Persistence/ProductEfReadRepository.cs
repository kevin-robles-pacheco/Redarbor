using Inventory.Application.Products;
using Inventory.Domain.Products;
using Microsoft.EntityFrameworkCore;
namespace Inventory.Infrastructure.Persistence;
public sealed class ProductEfReadRepository : IProductReadRepository
{
    private readonly InventoryDbContext _dbContext;
    public ProductEfReadRepository(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Product?> GetByIdAsync(GetProductQuery query, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(item => item.Id == query.ProductId, cancellationToken);

        return product;
    }
    public async Task<IReadOnlyList<Product>> ListAsync(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var products = _dbContext.Products.AsQueryable();
        if (query.CategoryId is Guid categoryId)
        {
            products = products.Where(product => product.CategoryId == categoryId);
        }
        var listed = await products.ToListAsync(cancellationToken);

        return listed;
    }
    public async Task<bool> ExistsByCategoryAsync(CategoryProductLookup lookup, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Products.AnyAsync(product => product.CategoryId == lookup.CategoryId, cancellationToken);

        return exists;
    }
    public async Task<bool> ExistsBySkuAsync(SkuLookup lookup, CancellationToken cancellationToken)
    {
        var sku = lookup.Sku.Trim();
        var products = _dbContext.Products.Where(product => product.Sku == sku);
        if (lookup.ProductId is Guid productId)
        {
            products = products.Where(product => product.Id != productId);
        }
        var exists = await products.AnyAsync(cancellationToken);

        return exists;
    }
}
