using Inventory.Domain.Movements;
namespace Inventory.Api.Contracts;
public static class MovementTypeParser
{
    public static MovementType Parse(string? value)
    {
        if (Enum.TryParse<MovementType>(value, true, out var movementType) && Enum.IsDefined(movementType))
        {

            return movementType;
        }
        throw new InvalidMovementTypeException();
    }
}
