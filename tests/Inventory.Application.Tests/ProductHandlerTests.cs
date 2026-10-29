using Inventory.Application.Categories;
using Inventory.Application.Movements;
using Inventory.Application.Products;
using Inventory.Application.Tests.Fakes;
using Inventory.Domain.Categories;
namespace Inventory.Application.Tests;
public sealed class ProductHandlerTests
{
    [Fact]
    public async Task Create_WhenCategoryDoesNotExist_ThrowsCategoryNotFoundException()
    {
        var writes = new FakeProductWriteRepository();
        var handler = new CreateProductHandler(new FakeCategoryReadRepository(), new FakeProductReadRepository(), writes);
        var command = new CreateProductCommand("Chair", null, "CHAIR-1", 10m, Guid.NewGuid(), DateTime.UtcNow);
        await Assert.ThrowsAsync<CategoryNotFoundException>(() => handler.Handle(command, CancellationToken.None));
        Assert.Null(writes.Added);
    }
    [Fact]
    public async Task Create_WhenSkuExists_ThrowsDuplicateSkuException()
    {
        var category = Category.Create(new CreateCategory("Furniture", null, DateTime.UtcNow));
        var categories = new FakeCategoryReadRepository { Stored = category };
        var products = new FakeProductReadRepository { SkuExists = true };
        var writes = new FakeProductWriteRepository();
        var handler = new CreateProductHandler(categories, products, writes);
        var command = new CreateProductCommand("Chair", null, "CHAIR-1", 10m, category.Id, DateTime.UtcNow);
        await Assert.ThrowsAsync<DuplicateSkuException>(() => handler.Handle(command, CancellationToken.None));
        Assert.Null(writes.Added);
    }
    [Fact]
    public async Task Create_AddsProductThroughWritePort()
    {
        var category = Category.Create(new CreateCategory("Furniture", null, DateTime.UtcNow));
        var categories = new FakeCategoryReadRepository { Stored = category };
        var writes = new FakeProductWriteRepository();
        var handler = new CreateProductHandler(categories, new FakeProductReadRepository(), writes);
        var command = new CreateProductCommand("Chair", null, "CHAIR-1", 10m, category.Id, DateTime.UtcNow);
        var response = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(0, response.QuantityOnHand);
        Assert.NotNull(writes.Added);
        Assert.Equal(category.Id, writes.Added.CategoryId);
    }
    [Fact]
    public async Task Delete_WhenMovementsExist_ThrowsProductHasMovementsException()
    {
        var categoryId = Guid.NewGuid();
        var product = Inventory.Domain.Products.Product.Create(new Inventory.Domain.Products.CreateProduct("Chair", null, "CHAIR-1", 10m, categoryId, DateTime.UtcNow));
        var products = new FakeProductReadRepository { Stored = product };
        var movements = new FakeInventoryMovementReadRepository { HasMovements = true };
        var writes = new FakeProductWriteRepository();
        var handler = new DeleteProductHandler(products, movements, writes);
        var command = new DeleteProductCommand(product.Id);
        await Assert.ThrowsAsync<ProductHasMovementsException>(() => handler.Handle(command, CancellationToken.None));
        Assert.False(writes.DeleteWasCalled);
    }
}
