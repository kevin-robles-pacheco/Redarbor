namespace Inventory.Domain.Categories;
public sealed class CreateCategory
{
    public CreateCategory(string name, string? description, DateTime createdAtUtc)
    {
        Name = name;
        Description = description;
        CreatedAtUtc = createdAtUtc;
    }
    public string Name { get; }
    public string? Description { get; }
    public DateTime CreatedAtUtc { get; }
}
