using Inventory.Application.Products;
using Inventory.Domain.Products;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace Inventory.Infrastructure.Tests;
public sealed class ProductEfReadRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProduct()
    {
        var product = CreateProduct("CHAIR-1");
        await using var context = CreateContext();
        context.Products.Add(product);
        await context.SaveChangesAsync();
        var repository = new ProductEfReadRepository(context);
        var found = await repository.GetByIdAsync(new GetProductQuery(product.Id), CancellationToken.None);
        Assert.NotNull(found);
        Assert.Equal(product.Sku, found.Sku);
    }
    [Fact]
    public async Task ListAsync_WhenCategoryFilterIsSet_ReturnsMatchingProducts()
    {
        var matching = CreateProduct("CHAIR-1");
        var other = Product.Create(new CreateProduct("Lamp", null, "LAMP-1", 5m, Guid.NewGuid(), DateTime.UtcNow));
        await using var context = CreateContext();
        context.Products.Add(matching);
        context.Products.Add(other);
        await context.SaveChangesAsync();
        var repository = new ProductEfReadRepository(context);
        var listed = await repository.ListAsync(new GetProductsQuery(matching.CategoryId), CancellationToken.None);
        Assert.Single(listed);
        Assert.Equal(matching.Id, listed[0].Id);
    }
    [Fact]
    public async Task ExistsBySkuAsync_WhenSkuBelongsToAnotherProduct_ReturnsTrue()
    {
        var product = CreateProduct("CHAIR-1");
        await using var context = CreateContext();
        context.Products.Add(product);
        await context.SaveChangesAsync();
        var repository = new ProductEfReadRepository(context);
        var exists = await repository.ExistsBySkuAsync(new SkuLookup("CHAIR-1", Guid.NewGuid()), CancellationToken.None);
        Assert.True(exists);
    }
    [Fact]
    public async Task ExistsByCategoryAsync_WhenProductsExist_ReturnsTrue()
    {
        var product = CreateProduct("CHAIR-1");
        await using var context = CreateContext();
        context.Products.Add(product);
        await context.SaveChangesAsync();
        var repository = new ProductEfReadRepository(context);
        var exists = await repository.ExistsByCategoryAsync(new CategoryProductLookup(product.CategoryId), CancellationToken.None);
        Assert.True(exists);
    }
    private static Product CreateProduct(string sku)
    {

        return Product.Create(new CreateProduct("Chair", null, sku, 10m, Guid.NewGuid(), DateTime.UtcNow));
    }
    private static InventoryDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        return new InventoryDbContext(options, allowWrites: true);
    }
}
