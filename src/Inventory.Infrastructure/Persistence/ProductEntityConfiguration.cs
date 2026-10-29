using Inventory.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Inventory.Infrastructure.Persistence;
public sealed class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(product => product.Id);
        builder.HasIndex(product => product.Sku).IsUnique();
        builder.Property(product => product.Id).HasColumnName("id").HasColumnType("uuid");
        builder.Property(product => product.Name).HasColumnName("name").HasColumnType("text");
        builder.Property(product => product.Description).HasColumnName("description").HasColumnType("text");
        builder.Property(product => product.Sku).HasColumnName("sku").HasColumnType("text");
        builder.Property(product => product.Price).HasColumnName("price").HasColumnType("numeric(18,2)");
        builder.Property(product => product.CategoryId).HasColumnName("category_id").HasColumnType("uuid");
        builder.Property(product => product.QuantityOnHand).HasColumnName("quantity_on_hand").HasColumnType("integer");
        builder.Property(product => product.IsActive).HasColumnName("is_active").HasColumnType("boolean");
        builder.Property(product => product.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamp with time zone");
    }
}
