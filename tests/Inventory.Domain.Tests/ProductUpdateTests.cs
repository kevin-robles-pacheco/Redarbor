using Inventory.Domain.Movements;
using Inventory.Domain.Products;
namespace Inventory.Domain.Tests;
public sealed class ProductUpdateTests
{
    [Fact]
    public void Update_WhenDetailsChange_KeepsIdentityAndStock()
    {
        var createdAt = new DateTime(2026, 10, 28, 12, 0, 0, DateTimeKind.Utc);
        var product = Product.Create(new CreateProduct("Chair", "Wood", "CHAIR-1", 10m, Guid.NewGuid(), createdAt));
        var movement = InventoryMovement.Create(new NewInventoryMovement(product.Id, MovementType.In, 2, createdAt, null));
        var stocked = product.Apply(movement);
        var categoryId = Guid.NewGuid();
        var updated = stocked.Update(new UpdateProduct("Desk", null, " DESK-1 ", 30m, categoryId));
        Assert.Equal(product.Id, updated.Id);
        Assert.Equal(2, updated.QuantityOnHand);
        Assert.Equal("Desk", updated.Name);
        Assert.Equal("DESK-1", updated.Sku);
        Assert.Equal(categoryId, updated.CategoryId);
        Assert.Equal(createdAt, updated.CreatedAtUtc);
    }
    [Fact]
    public void Update_WhenPriceIsNegative_ThrowsInvalidPriceException()
    {
        var product = Product.Create(new CreateProduct("Chair", null, "CHAIR-1", 10m, Guid.NewGuid(), DateTime.UtcNow));
        Assert.Throws<InvalidPriceException>(() => product.Update(new UpdateProduct("Chair", null, "CHAIR-1", -1m, product.CategoryId)));
    }
}
