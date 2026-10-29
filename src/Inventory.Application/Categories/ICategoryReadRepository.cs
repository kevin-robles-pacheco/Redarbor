using Inventory.Domain.Categories;
namespace Inventory.Application.Categories;
public interface ICategoryReadRepository
{
    Task<Category?> GetByIdAsync(GetCategoryQuery query, CancellationToken cancellationToken);
    Task<IReadOnlyList<Category>> ListAsync(GetCategoriesQuery query, CancellationToken cancellationToken);
}
