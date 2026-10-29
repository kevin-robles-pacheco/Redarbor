using Inventory.Application.Products;
using MediatR;
namespace Inventory.Application.Categories;
public sealed class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly ICategoryReadRepository _categoryReadRepository;
    private readonly IProductReadRepository _productReadRepository;
    private readonly ICategoryWriteRepository _categoryWriteRepository;
    public DeleteCategoryHandler(ICategoryReadRepository categoryReadRepository, IProductReadRepository productReadRepository, ICategoryWriteRepository categoryWriteRepository)
    {
        _categoryReadRepository = categoryReadRepository;
        _productReadRepository = productReadRepository;
        _categoryWriteRepository = categoryWriteRepository;
    }
    public async Task<Unit> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryReadRepository.GetByIdAsync(new GetCategoryQuery(command.CategoryId), cancellationToken);
        if (category is null)
        {
            throw new CategoryNotFoundException();
        }
        if (await _productReadRepository.ExistsByCategoryAsync(new CategoryProductLookup(command.CategoryId), cancellationToken))
        {
            throw new CategoryHasProductsException();
        }
        await _categoryWriteRepository.DeleteAsync(command, cancellationToken);

        return Unit.Value;
    }
}
