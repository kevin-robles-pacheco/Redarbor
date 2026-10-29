using System.Data;
using Inventory.Application.Persistence;
using Npgsql;
namespace Inventory.Infrastructure.Persistence;
public sealed class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    public NpgsqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }
    public IDbConnection OpenConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        return connection;
    }
}
