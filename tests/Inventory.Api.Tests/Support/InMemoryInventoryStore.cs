using Inventory.Application.Categories;
using Inventory.Application.Movements;
using Inventory.Application.Persistence;
using Inventory.Application.Products;
using Inventory.Domain.Categories;
using Inventory.Domain.Products;
namespace Inventory.Api.Tests.Support;
public sealed class InMemoryInventoryStore :
    ICategoryReadRepository,
    ICategoryWriteRepository,
    IProductReadRepository,
    IProductWriteRepository,
    IInventoryMovementReadRepository,
    IInventoryMovementWriter,
    IDatabaseSchemaInitializer,
    IDbConnectionFactory
{
    private readonly Dictionary<Guid, Category> _categories = new();
    private readonly Dictionary<Guid, Product> _products = new();
    private readonly HashSet<Guid> _productsWithMovements = new();
    public Task InitializeAsync(CancellationToken cancellationToken)
    {

        return Task.CompletedTask;
    }
    public System.Data.IDbConnection OpenConnection()
    {
        throw new InvalidOperationException("In-memory store does not open SQL connections.");
    }
    public Task<Category?> GetByIdAsync(GetCategoryQuery query, CancellationToken cancellationToken)
    {
        _categories.TryGetValue(query.CategoryId, out var category);

        return Task.FromResult(category);
    }
    public Task<IReadOnlyList<Category>> ListAsync(GetCategoriesQuery query, CancellationToken cancellationToken)
    {

        return Task.FromResult<IReadOnlyList<Category>>(_categories.Values.ToList());
    }
    public Task AddAsync(Category category, CancellationToken cancellationToken)
    {
        _categories[category.Id] = category;

        return Task.CompletedTask;
    }
    public Task UpdateAsync(Category category, CancellationToken cancellationToken)
    {
        _categories[category.Id] = category;

        return Task.CompletedTask;
    }
    public Task DeleteAsync(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        _categories.Remove(command.CategoryId);

        return Task.CompletedTask;
    }
    public Task<Product?> GetByIdAsync(GetProductQuery query, CancellationToken cancellationToken)
    {
        _products.TryGetValue(query.ProductId, out var product);

        return Task.FromResult(product);
    }
    public Task<IReadOnlyList<Product>> ListAsync(GetProductsQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<Product> products = _products.Values;
        if (query.CategoryId is Guid categoryId)
        {
            products = products.Where(product => product.CategoryId == categoryId);
        }

        return Task.FromResult<IReadOnlyList<Product>>(products.ToList());
    }
    public Task<bool> ExistsByCategoryAsync(CategoryProductLookup lookup, CancellationToken cancellationToken)
    {

        return Task.FromResult(_products.Values.Any(product => product.CategoryId == lookup.CategoryId));
    }
    public Task<bool> ExistsBySkuAsync(SkuLookup lookup, CancellationToken cancellationToken)
    {
        var exists = _products.Values.Any(product => product.Sku == lookup.Sku.Trim() && (lookup.ProductId is null || product.Id != lookup.ProductId));

        return Task.FromResult(exists);
    }
    public Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        _products[product.Id] = product;

        return Task.CompletedTask;
    }
    public Task UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        _products[product.Id] = product;

        return Task.CompletedTask;
    }
    public Task DeleteAsync(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        _products.Remove(command.ProductId);

        return Task.CompletedTask;
    }
    public Task<bool> ExistsForProductAsync(ProductMovementLookup lookup, CancellationToken cancellationToken)
    {

        return Task.FromResult(_productsWithMovements.Contains(lookup.ProductId));
    }
    public Task RegisterAsync(RegisterMovement registration, CancellationToken cancellationToken)
    {
        _products[registration.Product.Id] = registration.Product;
        _productsWithMovements.Add(registration.Product.Id);

        return Task.CompletedTask;
    }
}
