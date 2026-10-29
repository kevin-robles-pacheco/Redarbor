using Dapper;
using Inventory.Application.Categories;
using Inventory.Application.Persistence;
using Inventory.Domain.Categories;
namespace Inventory.Infrastructure.Persistence;
public sealed class CategoryDapperWriteRepository : ICategoryWriteRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public CategoryDapperWriteRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    public async Task AddAsync(Category category, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO categories (id, name, description, is_active, created_at_utc)
            VALUES (@Id, @Name, @Description, @IsActive, @CreatedAtUtc)
            """;
        using var connection = _connectionFactory.OpenConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, category, cancellationToken: cancellationToken));
    }
    public async Task UpdateAsync(Category category, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE categories
            SET name = @Name, description = @Description, is_active = @IsActive
            WHERE id = @Id
            """;
        using var connection = _connectionFactory.OpenConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, category, cancellationToken: cancellationToken));
    }
    public async Task DeleteAsync(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        const string sql = """
            DELETE FROM categories
            WHERE id = @CategoryId
            """;
        using var connection = _connectionFactory.OpenConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, command, cancellationToken: cancellationToken));
    }
}
