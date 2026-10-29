using MediatR;
namespace Inventory.Application.Categories;
public sealed class UpdateCategoryCommand : IRequest<CategoryResponse>
{
    public UpdateCategoryCommand(Guid categoryId, string name, string? description)
    {
        CategoryId = categoryId;
        Name = name;
        Description = description;
    }
    public Guid CategoryId { get; }
    public string Name { get; }
    public string? Description { get; }
}
