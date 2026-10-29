namespace Inventory.Domain.Movements;
public sealed class QuantityMustBePositiveException : Exception
{
    public QuantityMustBePositiveException()
        : base("Movement quantity must be greater than zero.")
    {
    }
}
