using Inventory.Application.Categories;
using Inventory.Domain.Categories;
using Microsoft.EntityFrameworkCore;
namespace Inventory.Infrastructure.Persistence;
public sealed class CategoryEfReadRepository : ICategoryReadRepository
{
    private readonly InventoryDbContext _dbContext;
    public CategoryEfReadRepository(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Category?> GetByIdAsync(GetCategoryQuery query, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(item => item.Id == query.CategoryId, cancellationToken);

        return category;
    }
    public async Task<IReadOnlyList<Category>> ListAsync(GetCategoriesQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        var categories = await _dbContext.Categories.ToListAsync(cancellationToken);

        return categories;
    }
}
