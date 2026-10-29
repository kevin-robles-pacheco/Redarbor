using Inventory.Application.Movements;
using MediatR;
namespace Inventory.Application.Products;
public sealed class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IProductReadRepository _productReadRepository;
    private readonly IInventoryMovementReadRepository _movementReadRepository;
    private readonly IProductWriteRepository _productWriteRepository;
    public DeleteProductHandler(IProductReadRepository productReadRepository, IInventoryMovementReadRepository movementReadRepository, IProductWriteRepository productWriteRepository)
    {
        _productReadRepository = productReadRepository;
        _movementReadRepository = movementReadRepository;
        _productWriteRepository = productWriteRepository;
    }
    public async Task<Unit> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product = await _productReadRepository.GetByIdAsync(new GetProductQuery(command.ProductId), cancellationToken);
        if (product is null)
        {
            throw new ProductNotFoundException();
        }
        if (await _movementReadRepository.ExistsForProductAsync(new ProductMovementLookup(command.ProductId), cancellationToken))
        {
            throw new ProductHasMovementsException();
        }
        await _productWriteRepository.DeleteAsync(command, cancellationToken);

        return Unit.Value;
    }
}
