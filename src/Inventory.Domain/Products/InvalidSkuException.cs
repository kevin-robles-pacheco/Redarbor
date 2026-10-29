namespace Inventory.Domain.Products;
public sealed class InvalidSkuException : Exception
{
    public InvalidSkuException()
        : base("Sku is required.")
    {
    }
}
