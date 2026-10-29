using MediatR;
namespace Inventory.Application.Products;
public sealed class GetProductsHandler : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductResponse>>
{
    private readonly IProductReadRepository _productReadRepository;
    public GetProductsHandler(IProductReadRepository productReadRepository)
    {
        _productReadRepository = productReadRepository;
    }
    public async Task<IReadOnlyList<ProductResponse>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var products = await _productReadRepository.ListAsync(query, cancellationToken);

        return ProductResponse.From(products);
    }
}
