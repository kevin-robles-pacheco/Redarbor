namespace Inventory.Application.Movements;
public sealed class ProductMovementLookup
{
    public ProductMovementLookup(Guid productId)
    {
        ProductId = productId;
    }
    public Guid ProductId { get; }
}
