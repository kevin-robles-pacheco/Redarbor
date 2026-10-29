namespace Inventory.Domain.Movements;
public sealed class ProductReferenceIsRequiredException : Exception
{
    public ProductReferenceIsRequiredException()
        : base("Movement requires a product.")
    {
    }
}
