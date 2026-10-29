namespace Inventory.Domain.Products;
public sealed class InvalidProductNameException : Exception
{
    public InvalidProductNameException()
        : base("Product name is required.")
    {
    }
}
