using MediatR;
namespace Inventory.Application.Categories;
public sealed class CreateCategoryCommand : IRequest<CategoryResponse>
{
    public CreateCategoryCommand(string name, string? description, DateTime createdAtUtc)
    {
        Name = name;
        Description = description;
        CreatedAtUtc = createdAtUtc;
    }
    public string Name { get; }
    public string? Description { get; }
    public DateTime CreatedAtUtc { get; }
}
