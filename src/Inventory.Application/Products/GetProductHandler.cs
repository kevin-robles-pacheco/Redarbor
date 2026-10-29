using MediatR;
namespace Inventory.Application.Products;
public sealed class GetProductHandler : IRequestHandler<GetProductQuery, ProductResponse>
{
    private readonly IProductReadRepository _productReadRepository;
    public GetProductHandler(IProductReadRepository productReadRepository)
    {
        _productReadRepository = productReadRepository;
    }
    public async Task<ProductResponse> Handle(GetProductQuery query, CancellationToken cancellationToken)
    {
        var product = await _productReadRepository.GetByIdAsync(query, cancellationToken);
        if (product is null)
        {
            throw new ProductNotFoundException();
        }

        return ProductResponse.From(product);
    }
}
