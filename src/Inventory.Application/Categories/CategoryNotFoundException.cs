namespace Inventory.Application.Categories;
public sealed class CategoryNotFoundException : Exception
{
    public CategoryNotFoundException()
        : base("Category was not found.")
    {
    }
}
