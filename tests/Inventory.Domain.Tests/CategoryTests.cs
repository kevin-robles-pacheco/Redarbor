using Inventory.Domain.Categories;
namespace Inventory.Domain.Tests;
public sealed class CategoryTests
{
    [Fact]
    public void Create_WithValidRequest_SetsActiveCategory()
    {
        var createdAt = new DateTime(2026, 10, 28, 12, 0, 0, DateTimeKind.Utc);
        var request = new CreateCategory("  Beverages  ", null, createdAt);
        var category = Category.Create(request);
        Assert.NotEqual(Guid.Empty, category.Id);
        Assert.Equal("Beverages", category.Name);
        Assert.Equal(string.Empty, category.Description);
        Assert.True(category.IsActive);
        Assert.Equal(createdAt, category.CreatedAtUtc);
    }
    [Fact]
    public void Create_WhenNameIsBlank_ThrowsInvalidCategoryNameException()
    {
        var createdAt = new DateTime(2026, 10, 28, 12, 0, 0, DateTimeKind.Utc);
        var request = new CreateCategory(" ", "Cold drinks", createdAt);
        Assert.Throws<InvalidCategoryNameException>(() => Category.Create(request));
    }
}
