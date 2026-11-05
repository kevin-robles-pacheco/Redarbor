using Inventory.Application.Categories;
using Inventory.Application.Products;
using Inventory.Application.Tests.Fakes;
using Inventory.Domain.Categories;
using Inventory.Domain.Products;
namespace Inventory.Application.Tests;
public sealed class ProductQueryHandlerTests
{
    [Fact]
    public async Task GetById_WhenProductExists_ReturnsResponse()
    {
        var product = CreateProductEntity();
        var handler = new GetProductHandler(new FakeProductReadRepository { Stored = product });
        var response = await handler.Handle(new GetProductQuery(product.Id), CancellationToken.None);
        Assert.Equal(product.Id, response.Id);
        Assert.Equal("Chair", response.Name);
    }
    [Fact]
    public async Task GetById_WhenProductIsMissing_ThrowsProductNotFoundException()
    {
        var handler = new GetProductHandler(new FakeProductReadRepository());
        await Assert.ThrowsAsync<ProductNotFoundException>(() => handler.Handle(new GetProductQuery(Guid.NewGuid()), CancellationToken.None));
    }
    [Fact]
    public async Task GetAll_ReturnsMappedProducts()
    {
        var product = CreateProductEntity();
        var handler = new GetProductsHandler(new FakeProductReadRepository { Stored = product });
        var response = await handler.Handle(new GetProductsQuery(null), CancellationToken.None);
        Assert.Single(response);
        Assert.Equal(product.Sku, response[0].Sku);
    }
    [Fact]
    public async Task Update_WhenProductExists_PersistsChanges()
    {
        var category = Category.Create(new CreateCategory("Furniture", null, DateTime.UtcNow));
        var product = Product.Create(new CreateProduct("Chair", null, "CHAIR-1", 10m, category.Id, DateTime.UtcNow));
        var writes = new FakeProductWriteRepository();
        var handler = new UpdateProductHandler(new FakeCategoryReadRepository { Stored = category }, new FakeProductReadRepository { Stored = product }, writes);
        var response = await handler.Handle(new UpdateProductCommand(product.Id, "Desk", "Oak", "DESK-1", 30m, category.Id), CancellationToken.None);
        Assert.Equal("Desk", response.Name);
        Assert.Equal("DESK-1", response.Sku);
        Assert.NotNull(writes.Updated);
    }
    [Fact]
    public async Task Update_WhenSkuExists_ThrowsDuplicateSkuException()
    {
        var category = Category.Create(new CreateCategory("Furniture", null, DateTime.UtcNow));
        var product = Product.Create(new CreateProduct("Chair", null, "CHAIR-1", 10m, category.Id, DateTime.UtcNow));
        var products = new FakeProductReadRepository { Stored = product, SkuExists = true };
        var handler = new UpdateProductHandler(new FakeCategoryReadRepository { Stored = category }, products, new FakeProductWriteRepository());
        await Assert.ThrowsAsync<DuplicateSkuException>(() => handler.Handle(new UpdateProductCommand(product.Id, "Chair", null, "CHAIR-2", 10m, category.Id), CancellationToken.None));
    }
    [Fact]
    public async Task Update_WhenProductIsMissing_ThrowsProductNotFoundException()
    {
        var category = Category.Create(new CreateCategory("Furniture", null, DateTime.UtcNow));
        var handler = new UpdateProductHandler(new FakeCategoryReadRepository { Stored = category }, new FakeProductReadRepository(), new FakeProductWriteRepository());
        await Assert.ThrowsAsync<ProductNotFoundException>(() => handler.Handle(new UpdateProductCommand(Guid.NewGuid(), "Chair", null, "CHAIR-1", 10m, category.Id), CancellationToken.None));
    }
    private static Product CreateProductEntity()
    {

        return Product.Create(new CreateProduct("Chair", null, "CHAIR-1", 10m, Guid.NewGuid(), DateTime.UtcNow));
    }
}
