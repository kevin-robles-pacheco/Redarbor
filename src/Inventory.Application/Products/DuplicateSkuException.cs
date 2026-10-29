namespace Inventory.Application.Products;
public sealed class DuplicateSkuException : Exception
{
    public DuplicateSkuException()
        : base("Sku is already in use.")
    {
    }
}
