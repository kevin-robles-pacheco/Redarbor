using Dapper;
using Inventory.Application.Persistence;
namespace Inventory.Infrastructure.Persistence;
public sealed class PostgresSchemaInitializer : IDatabaseSchemaInitializer
{
    private readonly IDbConnectionFactory _connectionFactory;
    public PostgresSchemaInitializer(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.OpenConnection();
        var command = new CommandDefinition(SchemaScript.Read(), cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }
}
