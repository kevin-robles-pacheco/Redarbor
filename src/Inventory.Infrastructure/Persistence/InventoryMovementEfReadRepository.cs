using Inventory.Application.Movements;
using Microsoft.EntityFrameworkCore;
namespace Inventory.Infrastructure.Persistence;
public sealed class InventoryMovementEfReadRepository : IInventoryMovementReadRepository
{
    private readonly InventoryDbContext _dbContext;
    public InventoryMovementEfReadRepository(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> ExistsForProductAsync(ProductMovementLookup lookup, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.InventoryMovements.AnyAsync(movement => movement.ProductId == lookup.ProductId, cancellationToken);

        return exists;
    }
}
