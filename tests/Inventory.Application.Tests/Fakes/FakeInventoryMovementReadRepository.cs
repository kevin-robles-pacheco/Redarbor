using Inventory.Application.Movements;
namespace Inventory.Application.Tests.Fakes;
public sealed class FakeInventoryMovementReadRepository : IInventoryMovementReadRepository
{
    public bool HasMovements { get; set; }
    public Task<bool> ExistsForProductAsync(ProductMovementLookup lookup, CancellationToken cancellationToken)
    {

        return Task.FromResult(HasMovements);
    }
}
