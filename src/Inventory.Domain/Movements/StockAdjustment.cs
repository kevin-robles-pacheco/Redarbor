namespace Inventory.Domain.Movements;
public sealed class StockAdjustment
{
    public StockAdjustment(int quantityOnHand, MovementType type, int quantity)
    {
        QuantityOnHand = quantityOnHand;
        Type = type;
        Quantity = quantity;
    }
    public int QuantityOnHand { get; }
    public MovementType Type { get; }
    public int Quantity { get; }
    public int Apply()
    {
        if (Quantity <= 0)
        {
            throw new QuantityMustBePositiveException();
        }
        if (Type == MovementType.Out && Quantity > QuantityOnHand)
        {
            throw new InsufficientStockException(QuantityOnHand, Quantity);
        }
        if (Type == MovementType.In)
        {

            return QuantityOnHand + Quantity;
        }
        if (Type == MovementType.Out)
        {

            return QuantityOnHand - Quantity;
        }
        throw new InvalidMovementTypeException();
    }
}
