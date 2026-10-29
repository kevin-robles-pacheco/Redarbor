using Inventory.Application.Categories;
using Inventory.Application.Tests.Fakes;
using Inventory.Domain.Categories;
namespace Inventory.Application.Tests;
public sealed class CategoryHandlerTests
{
    [Fact]
    public async Task Create_AddsCategoryThroughWritePort()
    {
        var writes = new FakeCategoryWriteRepository();
        var handler = new CreateCategoryHandler(writes);
        var createdAt = new DateTime(2026, 10, 28, 12, 0, 0, DateTimeKind.Utc);
        var command = new CreateCategoryCommand("Beverages", null, createdAt);
        var response = await handler.Handle(command, CancellationToken.None);
        Assert.Equal("Beverages", response.Name);
        Assert.NotNull(writes.Added);
        Assert.Equal(response.Id, writes.Added.Id);
    }
    [Fact]
    public async Task Delete_WhenProductsExist_ThrowsCategoryHasProductsException()
    {
        var category = Category.Create(new CreateCategory("Beverages", null, DateTime.UtcNow));
        var reads = new FakeCategoryReadRepository { Stored = category };
        var products = new FakeProductReadRepository { CategoryHasProducts = true };
        var writes = new FakeCategoryWriteRepository();
        var handler = new DeleteCategoryHandler(reads, products, writes);
        var command = new DeleteCategoryCommand(category.Id);
        await Assert.ThrowsAsync<CategoryHasProductsException>(() => handler.Handle(command, CancellationToken.None));
        Assert.False(writes.DeleteWasCalled);
    }
    [Fact]
    public async Task Delete_WhenCategoryIsMissing_ThrowsCategoryNotFoundException()
    {
        var handler = new DeleteCategoryHandler(new FakeCategoryReadRepository(), new FakeProductReadRepository(), new FakeCategoryWriteRepository());
        var command = new DeleteCategoryCommand(Guid.NewGuid());
        await Assert.ThrowsAsync<CategoryNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
