namespace Inventory.Domain.Categories;
public sealed class UpdateCategory
{
    public UpdateCategory(string name, string? description)
    {
        Name = name;
        Description = description;
    }
    public string Name { get; }
    public string? Description { get; }
}
