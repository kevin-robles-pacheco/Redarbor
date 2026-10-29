using Inventory.Domain.Movements;
namespace Inventory.Domain.Products;
public sealed class Product
{
    private Product(Guid id, string name, string description, string sku, decimal price, Guid categoryId, int quantityOnHand, bool isActive, DateTime createdAtUtc)
    {
        Id = id;
        Name = name;
        Description = description;
        Sku = sku;
        Price = price;
        CategoryId = categoryId;
        QuantityOnHand = quantityOnHand;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
    }
    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public string Sku { get; }
    public decimal Price { get; }
    public Guid CategoryId { get; }
    public int QuantityOnHand { get; }
    public bool IsActive { get; }
    public DateTime CreatedAtUtc { get; }
    public static Product Create(CreateProduct request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidProductNameException();
        }
        if (string.IsNullOrWhiteSpace(request.Sku))
        {
            throw new InvalidSkuException();
        }
        if (request.Price < 0)
        {
            throw new InvalidPriceException();
        }
        if (request.CategoryId == Guid.Empty)
        {
            throw new CategoryReferenceIsRequiredException();
        }
        var description = request.Description ?? string.Empty;

        return new Product(Guid.NewGuid(), request.Name.Trim(), description.Trim(), request.Sku.Trim(), request.Price, request.CategoryId, 0, true, request.CreatedAtUtc);
    }
    public Product Apply(InventoryMovement movement)
    {
        if (movement.ProductId != Id)
        {
            throw new MovementProductMismatchException();
        }
        var adjustment = new StockAdjustment(QuantityOnHand, movement.Type, movement.Quantity);
        var quantityOnHand = adjustment.Apply();

        return new Product(Id, Name, Description, Sku, Price, CategoryId, quantityOnHand, IsActive, CreatedAtUtc);
    }
    public Product Update(UpdateProduct request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidProductNameException();
        }
        if (string.IsNullOrWhiteSpace(request.Sku))
        {
            throw new InvalidSkuException();
        }
        if (request.Price < 0)
        {
            throw new InvalidPriceException();
        }
        if (request.CategoryId == Guid.Empty)
        {
            throw new CategoryReferenceIsRequiredException();
        }
        var description = request.Description ?? string.Empty;

        return new Product(Id, request.Name.Trim(), description.Trim(), request.Sku.Trim(), request.Price, request.CategoryId, QuantityOnHand, IsActive, CreatedAtUtc);
    }
}
