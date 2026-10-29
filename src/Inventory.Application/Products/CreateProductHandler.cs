using Inventory.Application.Categories;
using Inventory.Domain.Products;
using MediatR;
namespace Inventory.Application.Products;
public sealed class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductResponse>
{
    private readonly ICategoryReadRepository _categoryReadRepository;
    private readonly IProductReadRepository _productReadRepository;
    private readonly IProductWriteRepository _productWriteRepository;
    public CreateProductHandler(ICategoryReadRepository categoryReadRepository, IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository)
    {
        _categoryReadRepository = categoryReadRepository;
        _productReadRepository = productReadRepository;
        _productWriteRepository = productWriteRepository;
    }
    public async Task<ProductResponse> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryReadRepository.GetByIdAsync(new GetCategoryQuery(command.CategoryId), cancellationToken);
        if (category is null)
        {
            throw new CategoryNotFoundException();
        }
        if (await _productReadRepository.ExistsBySkuAsync(new SkuLookup(command.Sku, null), cancellationToken))
        {
            throw new DuplicateSkuException();
        }
        var request = new CreateProduct(command.Name, command.Description, command.Sku, command.Price, command.CategoryId, command.CreatedAtUtc);
        var product = Product.Create(request);
        await _productWriteRepository.AddAsync(product, cancellationToken);

        return ProductResponse.From(product);
    }
}
