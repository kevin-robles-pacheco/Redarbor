using MediatR;
namespace Inventory.Application.Categories;
public sealed class GetCategoriesQuery : IRequest<IReadOnlyList<CategoryResponse>>
{
}
