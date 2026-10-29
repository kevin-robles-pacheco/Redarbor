using Inventory.Domain.Categories;
using Inventory.Domain.Movements;
using Inventory.Domain.Products;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace Inventory.Infrastructure.Tests;
public sealed class PersistenceModelTests
{
    [Fact]
    public void Script_HasPrimaryKeysAndNoForeignKeys()
    {
        var sql = SchemaScript.Read();
        Assert.Contains("PRIMARY KEY", sql, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ux_products_sku", sql, StringComparison.Ordinal);
        Assert.DoesNotContain("FOREIGN KEY", sql, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("REFERENCES", sql, StringComparison.OrdinalIgnoreCase);
    }
    [Fact]
    public void Model_MapsEntitiesWithoutForeignKeys()
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>().UseNpgsql("Host=localhost;Database=inventory").Options;
        using var context = new InventoryDbContext(options);
        var entityTypes = context.Model.GetEntityTypes().Select(entityType => entityType.ClrType).ToArray();
        Assert.Contains(typeof(Category), entityTypes);
        Assert.Contains(typeof(Product), entityTypes);
        Assert.Contains(typeof(InventoryMovement), entityTypes);
        var foreignKeys = context.Model.GetEntityTypes().SelectMany(entityType => entityType.GetForeignKeys()).ToArray();
        Assert.Empty(foreignKeys);
        var productType = context.Model.FindEntityType(typeof(Product));
        var skuIndex = productType!.GetIndexes().Single(index => index.Properties.Any(property => property.Name == "Sku"));
        Assert.True(skuIndex.IsUnique);
    }
}
