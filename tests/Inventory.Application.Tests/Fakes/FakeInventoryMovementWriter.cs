using Inventory.Application.Movements;
namespace Inventory.Application.Tests.Fakes;
public sealed class FakeInventoryMovementWriter : IInventoryMovementWriter
{
    public RegisterMovement? Registration { get; private set; }
    public Task RegisterAsync(RegisterMovement registration, CancellationToken cancellationToken)
    {
        Registration = registration;

        return Task.CompletedTask;
    }
}
