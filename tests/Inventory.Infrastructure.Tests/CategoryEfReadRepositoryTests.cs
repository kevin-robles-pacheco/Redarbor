using Inventory.Application.Categories;
using Inventory.Domain.Categories;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace Inventory.Infrastructure.Tests;
public sealed class CategoryEfReadRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_WhenCategoryExists_ReturnsCategory()
    {
        var category = Category.Create(new CreateCategory("Furniture", "Wood", DateTime.UtcNow));
        await using var context = CreateContext();
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        var repository = new CategoryEfReadRepository(context);
        var found = await repository.GetByIdAsync(new GetCategoryQuery(category.Id), CancellationToken.None);
        Assert.NotNull(found);
        Assert.Equal(category.Name, found.Name);
    }
    [Fact]
    public async Task ListAsync_ReturnsAllCategories()
    {
        var category = Category.Create(new CreateCategory("Furniture", null, DateTime.UtcNow));
        await using var context = CreateContext();
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        var repository = new CategoryEfReadRepository(context);
        var listed = await repository.ListAsync(new GetCategoriesQuery(), CancellationToken.None);
        Assert.Contains(listed, item => item.Id == category.Id);
    }
    private static InventoryDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        return new InventoryDbContext(options, allowWrites: true);
    }
}
