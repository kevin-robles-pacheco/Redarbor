namespace Inventory.Domain.Categories;
public sealed class Category
{
    private Category(Guid id, string name, string description, bool isActive, DateTime createdAtUtc)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
    }
    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public bool IsActive { get; }
    public DateTime CreatedAtUtc { get; }
    public static Category Create(CreateCategory request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidCategoryNameException();
        }
        var description = request.Description ?? string.Empty;

        return new Category(Guid.NewGuid(), request.Name.Trim(), description.Trim(), true, request.CreatedAtUtc);
    }
    public Category Update(UpdateCategory request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidCategoryNameException();
        }
        var description = request.Description ?? string.Empty;

        return new Category(Id, request.Name.Trim(), description.Trim(), IsActive, CreatedAtUtc);
    }
}
