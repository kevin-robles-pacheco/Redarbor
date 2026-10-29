namespace Inventory.Domain.Categories;
public sealed class InvalidCategoryNameException : Exception
{
    public InvalidCategoryNameException()
        : base("Category name is required.")
    {
    }
}
