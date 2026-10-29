namespace Inventory.Domain.Movements;
public sealed class InvalidMovementTypeException : Exception
{
    public InvalidMovementTypeException()
        : base("Movement type is not supported.")
    {
    }
}
