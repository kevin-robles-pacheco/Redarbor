using Inventory.Domain.Categories;
using Inventory.Domain.Movements;
using Inventory.Domain.Products;
using Microsoft.EntityFrameworkCore;
namespace Inventory.Infrastructure.Persistence;
public sealed class InventoryDbContext : DbContext
{
    private readonly bool _allowWrites;
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : this(options, allowWrites: false)
    {
    }
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options, bool allowWrites)
        : base(options)
    {
        _allowWrites = allowWrites;
    }
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);
    }
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        EnsureWritesAreAllowed();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        EnsureWritesAreAllowed();

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    private void EnsureWritesAreAllowed()
    {
        if (!_allowWrites)
        {
            throw new InvalidOperationException("EF Core is limited to reads.");
        }
    }
}
