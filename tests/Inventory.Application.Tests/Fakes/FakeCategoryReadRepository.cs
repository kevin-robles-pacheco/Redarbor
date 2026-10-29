using Inventory.Application.Categories;
using Inventory.Domain.Categories;
namespace Inventory.Application.Tests.Fakes;
public sealed class FakeCategoryReadRepository : ICategoryReadRepository
{
    public Category? Stored { get; set; }
    public Task<Category?> GetByIdAsync(GetCategoryQuery query, CancellationToken cancellationToken)
    {

        return Task.FromResult(Stored);
    }
    public Task<IReadOnlyList<Category>> ListAsync(GetCategoriesQuery query, CancellationToken cancellationToken)
    {
        IReadOnlyList<Category> categories = Stored is null ? Array.Empty<Category>() : new[] { Stored };

        return Task.FromResult(categories);
    }
}
