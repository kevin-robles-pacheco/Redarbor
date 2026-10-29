namespace Inventory.Application.Products;
public sealed class CategoryProductLookup
{
    public CategoryProductLookup(Guid categoryId)
    {
        CategoryId = categoryId;
    }
    public Guid CategoryId { get; }
}
