namespace Inventory.Domain.Products;
public sealed class InvalidPriceException : Exception
{
    public InvalidPriceException()
        : base("Price cannot be negative.")
    {
    }
}
