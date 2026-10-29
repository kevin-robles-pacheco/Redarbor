using MediatR;
namespace Inventory.Application.Categories;
public sealed class GetCategoryHandler : IRequestHandler<GetCategoryQuery, CategoryResponse>
{
    private readonly ICategoryReadRepository _categoryReadRepository;
    public GetCategoryHandler(ICategoryReadRepository categoryReadRepository)
    {
        _categoryReadRepository = categoryReadRepository;
    }
    public async Task<CategoryResponse> Handle(GetCategoryQuery query, CancellationToken cancellationToken)
    {
        var category = await _categoryReadRepository.GetByIdAsync(query, cancellationToken);
        if (category is null)
        {
            throw new CategoryNotFoundException();
        }

        return CategoryResponse.From(category);
    }
}
