using Inventory.Domain.Categories;
using MediatR;
namespace Inventory.Application.Categories;
public sealed class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CategoryResponse>
{
    private readonly ICategoryWriteRepository _categoryWriteRepository;
    public CreateCategoryHandler(ICategoryWriteRepository categoryWriteRepository)
    {
        _categoryWriteRepository = categoryWriteRepository;
    }
    public async Task<CategoryResponse> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var request = new CreateCategory(command.Name, command.Description, command.CreatedAtUtc);
        var category = Category.Create(request);
        await _categoryWriteRepository.AddAsync(category, cancellationToken);

        return CategoryResponse.From(category);
    }
}
