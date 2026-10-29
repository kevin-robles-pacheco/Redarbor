using Dapper;
using Inventory.Application.Movements;
using Inventory.Application.Persistence;
using Inventory.Domain.Movements;
using Inventory.Domain.Products;
namespace Inventory.Infrastructure.Persistence;
public sealed class InventoryMovementDapperWriter : IInventoryMovementWriter
{
    private readonly IDbConnectionFactory _connectionFactory;
    public InventoryMovementDapperWriter(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    public async Task RegisterAsync(RegisterMovement registration, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.OpenConnection();
        using var transaction = connection.BeginTransaction();
        await InsertMovementAsync(connection, transaction, registration.Movement, cancellationToken);
        await UpdateStockAsync(connection, transaction, registration.Product, cancellationToken);
        transaction.Commit();
    }
    private static async Task InsertMovementAsync(System.Data.IDbConnection connection, System.Data.IDbTransaction transaction, InventoryMovement movement, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO inventory_movements (id, product_id, type, quantity, occurred_at_utc, notes)
            VALUES (@Id, @ProductId, @Type, @Quantity, @OccurredAtUtc, @Notes)
            """;
        var insert = new MovementInsert(movement);
        await connection.ExecuteAsync(new CommandDefinition(sql, insert, transaction, cancellationToken: cancellationToken));
    }
    private static async Task UpdateStockAsync(System.Data.IDbConnection connection, System.Data.IDbTransaction transaction, Product product, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE products
            SET quantity_on_hand = @QuantityOnHand
            WHERE id = @Id
            """;
        await connection.ExecuteAsync(new CommandDefinition(sql, product, transaction, cancellationToken: cancellationToken));
    }
}
