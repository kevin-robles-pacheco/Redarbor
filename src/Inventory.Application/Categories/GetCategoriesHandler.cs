using MediatR;
namespace Inventory.Application.Categories;
public sealed class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryResponse>>
{
    private readonly ICategoryReadRepository _categoryReadRepository;
    public GetCategoriesHandler(ICategoryReadRepository categoryReadRepository)
    {
        _categoryReadRepository = categoryReadRepository;
    }
    public async Task<IReadOnlyList<CategoryResponse>> Handle(GetCategoriesQuery query, CancellationToken cancellationToken)
    {
        var categories = await _categoryReadRepository.ListAsync(query, cancellationToken);

        return CategoryResponse.From(categories);
    }
}
