namespace Inventory.Application.Persistence;
public interface IDatabaseSchemaInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken);
}
