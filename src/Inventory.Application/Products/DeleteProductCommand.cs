using MediatR;
namespace Inventory.Application.Products;
public sealed class DeleteProductCommand : IRequest<Unit>
{
    public DeleteProductCommand(Guid productId)
    {
        ProductId = productId;
    }
    public Guid ProductId { get; }
}
