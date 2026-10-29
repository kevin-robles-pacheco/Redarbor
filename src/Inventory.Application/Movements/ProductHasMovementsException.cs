namespace Inventory.Application.Movements;
public sealed class ProductHasMovementsException : Exception
{
    public ProductHasMovementsException()
        : base("Product cannot be deleted because it has inventory movements.")
    {
    }
}
