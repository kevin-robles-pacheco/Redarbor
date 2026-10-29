using Inventory.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Inventory.Infrastructure.Persistence;
public sealed class CategoryEntityConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(category => category.Id);
        builder.Property(category => category.Id).HasColumnName("id").HasColumnType("uuid");
        builder.Property(category => category.Name).HasColumnName("name").HasColumnType("text");
        builder.Property(category => category.Description).HasColumnName("description").HasColumnType("text");
        builder.Property(category => category.IsActive).HasColumnName("is_active").HasColumnType("boolean");
        builder.Property(category => category.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamp with time zone");
    }
}
