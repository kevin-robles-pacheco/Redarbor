using MediatR;
namespace Inventory.Application.Products;
public sealed class GetProductsQuery : IRequest<IReadOnlyList<ProductResponse>>
{
    public GetProductsQuery(Guid? categoryId)
    {
        CategoryId = categoryId;
    }
    public Guid? CategoryId { get; }
}
