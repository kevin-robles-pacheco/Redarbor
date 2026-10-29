namespace Inventory.Application.Products;
public sealed class ProductNotFoundException : Exception
{
    public ProductNotFoundException()
        : base("Product was not found.")
    {
    }
}
