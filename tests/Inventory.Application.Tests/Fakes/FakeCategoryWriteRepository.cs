using Inventory.Application.Categories;
using Inventory.Domain.Categories;
namespace Inventory.Application.Tests.Fakes;
public sealed class FakeCategoryWriteRepository : ICategoryWriteRepository
{
    public Category? Added { get; private set; }
    public Category? Updated { get; private set; }
    public bool DeleteWasCalled { get; private set; }
    public Task AddAsync(Category category, CancellationToken cancellationToken)
    {
        Added = category;

        return Task.CompletedTask;
    }
    public Task UpdateAsync(Category category, CancellationToken cancellationToken)
    {
        Updated = category;

        return Task.CompletedTask;
    }
    public Task DeleteAsync(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        DeleteWasCalled = true;

        return Task.CompletedTask;
    }
}
