using Inventory.Application.Products;
using Inventory.Domain.Products;
namespace Inventory.Application.Tests.Fakes;
public sealed class FakeProductWriteRepository : IProductWriteRepository
{
    public Product? Added { get; private set; }
    public bool DeleteWasCalled { get; private set; }
    public Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        Added = product;

        return Task.CompletedTask;
    }
    public Task UpdateAsync(Product product, CancellationToken cancellationToken)
    {

        return Task.CompletedTask;
    }
    public Task DeleteAsync(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        DeleteWasCalled = true;

        return Task.CompletedTask;
    }
}
