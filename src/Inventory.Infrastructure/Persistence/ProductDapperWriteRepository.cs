using Dapper;
using Inventory.Application.Persistence;
using Inventory.Application.Products;
using Inventory.Domain.Products;
namespace Inventory.Infrastructure.Persistence;
public sealed class ProductDapperWriteRepository : IProductWriteRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public ProductDapperWriteRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO products (id, name, description, sku, price, category_id, quantity_on_hand, is_active, created_at_utc)
            VALUES (@Id, @Name, @Description, @Sku, @Price, @CategoryId, @QuantityOnHand, @IsActive, @CreatedAtUtc)
            """;
        using var connection = _connectionFactory.OpenConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, product, cancellationToken: cancellationToken));
    }
    public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE products
            SET name = @Name, description = @Description, sku = @Sku, price = @Price, category_id = @CategoryId, is_active = @IsActive
            WHERE id = @Id
            """;
        using var connection = _connectionFactory.OpenConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, product, cancellationToken: cancellationToken));
    }
    public async Task DeleteAsync(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        const string sql = """
            DELETE FROM products
            WHERE id = @ProductId
            """;
        using var connection = _connectionFactory.OpenConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, command, cancellationToken: cancellationToken));
    }
}
