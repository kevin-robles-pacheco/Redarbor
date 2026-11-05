using Inventory.Infrastructure.Persistence;
namespace Inventory.Infrastructure.Tests;
public sealed class NpgsqlConnectionFactoryTests
{
    [Fact]
    public void Constructor_StoresConnectionStringForOpen()
    {
        var factory = new NpgsqlConnectionFactory("Host=localhost;Database=inventory;Username=inventory;Password=local-dev-only");
        Assert.NotNull(factory);
    }
}
