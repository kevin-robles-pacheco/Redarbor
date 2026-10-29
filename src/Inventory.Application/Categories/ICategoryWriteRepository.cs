using Inventory.Domain.Categories;
namespace Inventory.Application.Categories;
public interface ICategoryWriteRepository
{
    Task AddAsync(Category category, CancellationToken cancellationToken);
    Task UpdateAsync(Category category, CancellationToken cancellationToken);
    Task DeleteAsync(DeleteCategoryCommand command, CancellationToken cancellationToken);
}
