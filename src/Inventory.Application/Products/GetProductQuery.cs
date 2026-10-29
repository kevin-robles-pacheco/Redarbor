using MediatR;
namespace Inventory.Application.Products;
public sealed class GetProductQuery : IRequest<ProductResponse>
{
    public GetProductQuery(Guid productId)
    {
        ProductId = productId;
    }
    public Guid ProductId { get; }
}
