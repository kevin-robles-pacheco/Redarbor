using Inventory.Application.Products;
using Inventory.Domain.Movements;
using Inventory.Domain.Products;
using MediatR;
namespace Inventory.Application.Movements;
public sealed class RegisterInventoryMovementHandler : IRequestHandler<RegisterInventoryMovementCommand, ProductResponse>
{
    private readonly IProductReadRepository _productReadRepository;
    private readonly IInventoryMovementWriter _inventoryMovementWriter;
    public RegisterInventoryMovementHandler(IProductReadRepository productReadRepository, IInventoryMovementWriter inventoryMovementWriter)
    {
        _productReadRepository = productReadRepository;
        _inventoryMovementWriter = inventoryMovementWriter;
    }
    public async Task<ProductResponse> Handle(RegisterInventoryMovementCommand command, CancellationToken cancellationToken)
    {
        var product = await _productReadRepository.GetByIdAsync(new GetProductQuery(command.ProductId), cancellationToken);
        if (product is null)
        {
            throw new ProductNotFoundException();
        }
        var request = new NewInventoryMovement(product.Id, command.Type, command.Quantity, command.OccurredAtUtc, command.Notes);
        var movement = InventoryMovement.Create(request);
        var updated = product.Apply(movement);
        await _inventoryMovementWriter.RegisterAsync(new RegisterMovement(updated, movement), cancellationToken);

        return ProductResponse.From(updated);
    }
}
