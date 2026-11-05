using Inventory.Application.Movements;
using Inventory.Domain.Movements;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace Inventory.Infrastructure.Tests;
public sealed class InventoryMovementEfReadRepositoryTests
{
    [Fact]
    public async Task ExistsForProductAsync_WhenMovementExists_ReturnsTrue()
    {
        var productId = Guid.NewGuid();
        var movement = InventoryMovement.Create(new NewInventoryMovement(productId, MovementType.In, 2, DateTime.UtcNow, null));
        await using var context = CreateContext();
        context.InventoryMovements.Add(movement);
        await context.SaveChangesAsync();
        var repository = new InventoryMovementEfReadRepository(context);
        var exists = await repository.ExistsForProductAsync(new ProductMovementLookup(productId), CancellationToken.None);
        Assert.True(exists);
    }
    [Fact]
    public async Task ExistsForProductAsync_WhenNoMovement_ReturnsFalse()
    {
        await using var context = CreateContext();
        var repository = new InventoryMovementEfReadRepository(context);
        var exists = await repository.ExistsForProductAsync(new ProductMovementLookup(Guid.NewGuid()), CancellationToken.None);
        Assert.False(exists);
    }
    private static InventoryDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        return new InventoryDbContext(options, allowWrites: true);
    }
}
