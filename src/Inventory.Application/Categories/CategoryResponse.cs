using Inventory.Domain.Categories;
namespace Inventory.Application.Categories;
public sealed class CategoryResponse
{
    public CategoryResponse(Guid id, string name, string description, bool isActive, DateTime createdAtUtc)
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
    public static CategoryResponse From(Category category)
    {

        return new CategoryResponse(category.Id, category.Name, category.Description, category.IsActive, category.CreatedAtUtc);
    }
    public static IReadOnlyList<CategoryResponse> From(IReadOnlyList<Category> categories)
    {
        var responses = new List<CategoryResponse>(categories.Count);
        foreach (var category in categories)
        {
            responses.Add(From(category));
        }

        return responses;
    }
}
