using MediatR;
namespace Inventory.Application.Categories;
public sealed class DeleteCategoryCommand : IRequest<Unit>
{
    public DeleteCategoryCommand(Guid categoryId)
    {
        CategoryId = categoryId;
    }
    public Guid CategoryId { get; }
}
