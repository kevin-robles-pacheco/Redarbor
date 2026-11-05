using Inventory.Application.Categories;
using Inventory.Application.Tests.Fakes;
using Inventory.Domain.Categories;
namespace Inventory.Application.Tests;
public sealed class CategoryQueryHandlerTests
{
    [Fact]
    public async Task GetById_WhenCategoryExists_ReturnsResponse()
    {
        var category = Category.Create(new CreateCategory("Furniture", "Wood", DateTime.UtcNow));
        var handler = new GetCategoryHandler(new FakeCategoryReadRepository { Stored = category });
        var response = await handler.Handle(new GetCategoryQuery(category.Id), CancellationToken.None);
        Assert.Equal(category.Id, response.Id);
        Assert.Equal("Furniture", response.Name);
    }
    [Fact]
    public async Task GetById_WhenCategoryIsMissing_ThrowsCategoryNotFoundException()
    {
        var handler = new GetCategoryHandler(new FakeCategoryReadRepository());
        await Assert.ThrowsAsync<CategoryNotFoundException>(() => handler.Handle(new GetCategoryQuery(Guid.NewGuid()), CancellationToken.None));
    }
    [Fact]
    public async Task GetAll_ReturnsMappedCategories()
    {
        var category = Category.Create(new CreateCategory("Furniture", null, DateTime.UtcNow));
        var handler = new GetCategoriesHandler(new FakeCategoryReadRepository { Stored = category });
        var response = await handler.Handle(new GetCategoriesQuery(), CancellationToken.None);
        Assert.Single(response);
        Assert.Equal(category.Id, response[0].Id);
    }
    [Fact]
    public async Task Update_WhenCategoryExists_PersistsChanges()
    {
        var category = Category.Create(new CreateCategory("Furniture", null, DateTime.UtcNow));
        var writes = new FakeCategoryWriteRepository();
        var handler = new UpdateCategoryHandler(new FakeCategoryReadRepository { Stored = category }, writes);
        var response = await handler.Handle(new UpdateCategoryCommand(category.Id, "Office", "Desks"), CancellationToken.None);
        Assert.Equal("Office", response.Name);
        Assert.NotNull(writes.Updated);
        Assert.Equal("Office", writes.Updated.Name);
    }
    [Fact]
    public async Task Update_WhenCategoryIsMissing_ThrowsCategoryNotFoundException()
    {
        var handler = new UpdateCategoryHandler(new FakeCategoryReadRepository(), new FakeCategoryWriteRepository());
        await Assert.ThrowsAsync<CategoryNotFoundException>(() => handler.Handle(new UpdateCategoryCommand(Guid.NewGuid(), "Office", null), CancellationToken.None));
    }
}
