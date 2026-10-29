using Inventory.Domain.Movements;
namespace Inventory.Domain.Tests;
public sealed class StockAdjustmentTests
{
    [Fact]
    public void Apply_WhenMovementIsIn_IncreasesQuantityOnHand()
    {
        var adjustment = new StockAdjustment(5, MovementType.In, 3);
        Assert.Equal(8, adjustment.Apply());
    }
    [Fact]
    public void Apply_WhenMovementIsOut_DecreasesQuantityOnHand()
    {
        var adjustment = new StockAdjustment(5, MovementType.Out, 2);
        Assert.Equal(3, adjustment.Apply());
    }
    [Fact]
    public void Apply_WhenOutQuantityEqualsStock_SetsQuantityOnHandToZero()
    {
        var adjustment = new StockAdjustment(5, MovementType.Out, 5);
        Assert.Equal(0, adjustment.Apply());
    }
    [Fact]
    public void Apply_WhenOutQuantityExceedsStock_ThrowsInsufficientStockException()
    {
        var adjustment = new StockAdjustment(5, MovementType.Out, 6);
        var exception = Assert.Throws<InsufficientStockException>(() => adjustment.Apply());
        Assert.Equal(5, exception.QuantityOnHand);
        Assert.Equal(6, exception.RequestedQuantity);
    }
    [Fact]
    public void Apply_WhenQuantityIsNotPositive_ThrowsQuantityMustBePositiveException()
    {
        var adjustment = new StockAdjustment(5, MovementType.In, 0);
        Assert.Throws<QuantityMustBePositiveException>(() => adjustment.Apply());
    }
}
