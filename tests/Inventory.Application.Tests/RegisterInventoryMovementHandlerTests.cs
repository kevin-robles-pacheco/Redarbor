using Inventory.Application.Movements;
using Inventory.Application.Products;
using Inventory.Application.Tests.Fakes;
using Inventory.Domain.Movements;
using Inventory.Domain.Products;
namespace Inventory.Application.Tests;
public sealed class RegisterInventoryMovementHandlerTests
{
    [Fact]
    public async Task Handle_CallsWriterWithUpdatedStock()
    {
        var product = Product.Create(new CreateProduct("Chair", null, "CHAIR-1", 10m, Guid.NewGuid(), DateTime.UtcNow));
        var products = new FakeProductReadRepository { Stored = product };
        var writer = new FakeInventoryMovementWriter();
        var handler = new RegisterInventoryMovementHandler(products, writer);
        var command = new RegisterInventoryMovementCommand(product.Id, MovementType.In, 4, DateTime.UtcNow, "initial");
        var response = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(4, response.QuantityOnHand);
        Assert.NotNull(writer.Registration);
        Assert.Equal(4, writer.Registration.Product.QuantityOnHand);
        Assert.Equal(product.Id, writer.Registration.Movement.ProductId);
        Assert.Equal(MovementType.In, writer.Registration.Movement.Type);
    }
    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ThrowsProductNotFoundException()
    {
        var writer = new FakeInventoryMovementWriter();
        var handler = new RegisterInventoryMovementHandler(new FakeProductReadRepository(), writer);
        var command = new RegisterInventoryMovementCommand(Guid.NewGuid(), MovementType.Out, 1, DateTime.UtcNow, null);
        await Assert.ThrowsAsync<ProductNotFoundException>(() => handler.Handle(command, CancellationToken.None));
        Assert.Null(writer.Registration);
    }
}
