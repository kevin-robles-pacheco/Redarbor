namespace Inventory.Application.Movements;
public interface IInventoryMovementReadRepository
{
    Task<bool> ExistsForProductAsync(ProductMovementLookup lookup, CancellationToken cancellationToken);
}
