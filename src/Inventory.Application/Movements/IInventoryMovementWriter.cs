namespace Inventory.Application.Movements;
public interface IInventoryMovementWriter
{
    Task RegisterAsync(RegisterMovement registration, CancellationToken cancellationToken);
}
