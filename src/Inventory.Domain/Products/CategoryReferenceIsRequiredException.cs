namespace Inventory.Domain.Products;
public sealed class CategoryReferenceIsRequiredException : Exception
{
    public CategoryReferenceIsRequiredException()
        : base("Product requires a category.")
    {
    }
}
