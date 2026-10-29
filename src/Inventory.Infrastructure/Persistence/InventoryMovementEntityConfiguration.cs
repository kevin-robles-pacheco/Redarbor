using Inventory.Domain.Movements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Inventory.Infrastructure.Persistence;
public sealed class InventoryMovementEntityConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.ToTable("inventory_movements");
        builder.HasKey(movement => movement.Id);
        builder.Property(movement => movement.Id).HasColumnName("id").HasColumnType("uuid");
        builder.Property(movement => movement.ProductId).HasColumnName("product_id").HasColumnType("uuid");
        builder.Property(movement => movement.Type).HasColumnName("type").HasColumnType("integer").HasConversion<int>();
        builder.Property(movement => movement.Quantity).HasColumnName("quantity").HasColumnType("integer");
        builder.Property(movement => movement.OccurredAtUtc).HasColumnName("occurred_at_utc").HasColumnType("timestamp with time zone");
        builder.Property(movement => movement.Notes).HasColumnName("notes").HasColumnType("text");
    }
}
