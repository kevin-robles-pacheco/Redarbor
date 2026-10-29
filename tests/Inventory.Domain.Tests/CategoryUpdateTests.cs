using Inventory.Domain.Categories;
namespace Inventory.Domain.Tests;
public sealed class CategoryUpdateTests
{
    [Fact]
    public void Update_WhenNameChanges_KeepsIdentity()
    {
        var createdAt = new DateTime(2026, 10, 28, 12, 0, 0, DateTimeKind.Utc);
        var category = Category.Create(new CreateCategory("Beverages", "Cold", createdAt));
        var updated = category.Update(new UpdateCategory("  Drinks  ", null));
        Assert.Equal(category.Id, updated.Id);
        Assert.Equal("Drinks", updated.Name);
        Assert.Equal(string.Empty, updated.Description);
        Assert.Equal(createdAt, updated.CreatedAtUtc);
    }
    [Fact]
    public void Update_WhenNameIsBlank_ThrowsInvalidCategoryNameException()
    {
        var category = Category.Create(new CreateCategory("Beverages", null, DateTime.UtcNow));
        Assert.Throws<InvalidCategoryNameException>(() => category.Update(new UpdateCategory(" ", "Cold")));
    }
}
