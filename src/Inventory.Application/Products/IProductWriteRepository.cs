using Inventory.Domain.Products;
namespace Inventory.Application.Products;
public interface IProductWriteRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken);
    Task UpdateAsync(Product product, CancellationToken cancellationToken);
    Task DeleteAsync(DeleteProductCommand command, CancellationToken cancellationToken);
}
