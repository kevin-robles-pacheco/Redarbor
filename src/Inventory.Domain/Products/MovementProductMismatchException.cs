namespace Inventory.Domain.Products;
public sealed class MovementProductMismatchException : Exception
{
    public MovementProductMismatchException()
        : base("Movement belongs to another product.")
    {
    }
}
