using MediatR;
namespace Inventory.Application.Categories;
public sealed class GetCategoryQuery : IRequest<CategoryResponse>
{
    public GetCategoryQuery(Guid categoryId)
    {
        CategoryId = categoryId;
    }
    public Guid CategoryId { get; }
}
