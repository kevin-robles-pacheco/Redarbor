namespace Inventory.Domain.Movements;
public sealed class InsufficientStockException : Exception
{
    public InsufficientStockException(int quantityOnHand, int requestedQuantity)
        : base("Stock cannot cover the outgoing movement.")
    {
        QuantityOnHand = quantityOnHand;
        RequestedQuantity = requestedQuantity;
    }
    public int QuantityOnHand { get; }
    public int RequestedQuantity { get; }
}
