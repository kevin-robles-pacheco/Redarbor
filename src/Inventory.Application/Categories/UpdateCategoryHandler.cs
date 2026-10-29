using Inventory.Domain.Categories;
using MediatR;
namespace Inventory.Application.Categories;
public sealed class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, CategoryResponse>
{
    private readonly ICategoryReadRepository _categoryReadRepository;
    private readonly ICategoryWriteRepository _categoryWriteRepository;
    public UpdateCategoryHandler(ICategoryReadRepository categoryReadRepository, ICategoryWriteRepository categoryWriteRepository)
    {
        _categoryReadRepository = categoryReadRepository;
        _categoryWriteRepository = categoryWriteRepository;
    }
    public async Task<CategoryResponse> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryReadRepository.GetByIdAsync(new GetCategoryQuery(command.CategoryId), cancellationToken);
        if (category is null)
        {
            throw new CategoryNotFoundException();
        }
        var updated = category.Update(new UpdateCategory(command.Name, command.Description));
        await _categoryWriteRepository.UpdateAsync(updated, cancellationToken);

        return CategoryResponse.From(updated);
    }
}
