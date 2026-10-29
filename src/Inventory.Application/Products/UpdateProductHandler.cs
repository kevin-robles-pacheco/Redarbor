using Inventory.Application.Categories;
using Inventory.Domain.Products;
using MediatR;
namespace Inventory.Application.Products;
public sealed class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ProductResponse>
{
    private readonly ICategoryReadRepository _categoryReadRepository;
    private readonly IProductReadRepository _productReadRepository;
    private readonly IProductWriteRepository _productWriteRepository;
    public UpdateProductHandler(ICategoryReadRepository categoryReadRepository, IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository)
    {
        _categoryReadRepository = categoryReadRepository;
        _productReadRepository = productReadRepository;
        _productWriteRepository = productWriteRepository;
    }
    public async Task<ProductResponse> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await _productReadRepository.GetByIdAsync(new GetProductQuery(command.ProductId), cancellationToken);
        if (product is null)
        {
            throw new ProductNotFoundException();
        }
        var category = await _categoryReadRepository.GetByIdAsync(new GetCategoryQuery(command.CategoryId), cancellationToken);
        if (category is null)
        {
            throw new CategoryNotFoundException();
        }
        if (await _productReadRepository.ExistsBySkuAsync(new SkuLookup(command.Sku, command.ProductId), cancellationToken))
        {
            throw new DuplicateSkuException();
        }
        var updated = product.Update(new UpdateProduct(command.Name, command.Description, command.Sku, command.Price, command.CategoryId));
        await _productWriteRepository.UpdateAsync(updated, cancellationToken);

        return ProductResponse.From(updated);
    }
}
