namespace Inventory.Application.Categories;
public sealed class CategoryHasProductsException : Exception
{
    public CategoryHasProductsException()
        : base("Category cannot be deleted because products reference it.")
    {
    }
}
