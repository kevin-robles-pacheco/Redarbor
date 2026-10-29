using Inventory.Domain.Movements;
namespace Inventory.Domain.Tests;
public sealed class InventoryMovementTests
{
    [Fact]
    public void Create_WithValidRequest_KeepsMovementData()
    {
        var productId = Guid.NewGuid();
        var occurredAt = new DateTime(2026, 10, 28, 12, 0, 0, DateTimeKind.Utc);
        var request = new NewInventoryMovement(productId, MovementType.In, 4, occurredAt, "  initial  ");
        var movement = InventoryMovement.Create(request);
        Assert.Equal(productId, movement.ProductId);
        Assert.Equal(MovementType.In, movement.Type);
        Assert.Equal(4, movement.Quantity);
        Assert.Equal(occurredAt, movement.OccurredAtUtc);
        Assert.Equal("initial", movement.Notes);
    }
    [Fact]
    public void Create_WhenQuantityIsNotPositive_ThrowsQuantityMustBePositiveException()
    {
        var request = new NewInventoryMovement(Guid.NewGuid(), MovementType.Out, 0, DateTime.UtcNow, null);
        Assert.Throws<QuantityMustBePositiveException>(() => InventoryMovement.Create(request));
    }
    [Fact]
    public void Create_WhenProductIdIsEmpty_ThrowsProductReferenceIsRequiredException()
    {
        var request = new NewInventoryMovement(Guid.Empty, MovementType.In, 1, DateTime.UtcNow, null);
        Assert.Throws<ProductReferenceIsRequiredException>(() => InventoryMovement.Create(request));
    }
    [Fact]
    public void Create_WhenTypeIsUndefined_ThrowsInvalidMovementTypeException()
    {
        var request = new NewInventoryMovement(Guid.NewGuid(), (MovementType)9, 1, DateTime.UtcNow, null);
        Assert.Throws<InvalidMovementTypeException>(() => InventoryMovement.Create(request));
    }
}
