using Inventory.Domain.Movements;
namespace Inventory.Domain.Products;
public sealed class Product
{
    private Product()
    {
    }
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Sku { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public Guid CategoryId { get; private set; }
    public int QuantityOnHand { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public static Product Create(CreateProduct request)
    {
        EnsureCreateIsValid(request);
        var description = request.Description ?? string.Empty;
        var state = new ProductState
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = description.Trim(),
            Sku = request.Sku.Trim(),
            Price = request.Price,
            CategoryId = request.CategoryId,
            QuantityOnHand = 0,
            IsActive = true,
            CreatedAtUtc = request.CreatedAtUtc
        };

        return FromState(state);
    }
    public Product Apply(InventoryMovement movement)
    {
        if (movement.ProductId != Id)
        {
            throw new MovementProductMismatchException();
        }
        var adjustment = new StockAdjustment(QuantityOnHand, movement.Type, movement.Quantity);
        var state = CaptureState();
        state.QuantityOnHand = adjustment.Apply();

        return FromState(state);
    }
    public Product Update(UpdateProduct request)
    {
        EnsureUpdateIsValid(request);
        var description = request.Description ?? string.Empty;
        var state = CaptureState();
        state.Name = request.Name.Trim();
        state.Description = description.Trim();
        state.Sku = request.Sku.Trim();
        state.Price = request.Price;
        state.CategoryId = request.CategoryId;

        return FromState(state);
    }
    private ProductState CaptureState()
    {
        var state = new ProductState
        {
            Id = Id,
            Name = Name,
            Description = Description,
            Sku = Sku,
            Price = Price,
            CategoryId = CategoryId,
            QuantityOnHand = QuantityOnHand,
            IsActive = IsActive,
            CreatedAtUtc = CreatedAtUtc
        };

        return state;
    }
    private static Product FromState(ProductState state)
    {
        var product = new Product
        {
            Id = state.Id,
            Name = state.Name,
            Description = state.Description,
            Sku = state.Sku,
            Price = state.Price,
            CategoryId = state.CategoryId,
            QuantityOnHand = state.QuantityOnHand,
            IsActive = state.IsActive,
            CreatedAtUtc = state.CreatedAtUtc
        };

        return product;
    }
    private static void EnsureCreateIsValid(CreateProduct request)
    {
        EnsureCatalogIsValid(request.Name, request.Sku, request.Price, request.CategoryId);
    }
    private static void EnsureUpdateIsValid(UpdateProduct request)
    {
        EnsureCatalogIsValid(request.Name, request.Sku, request.Price, request.CategoryId);
    }
    private static void EnsureCatalogIsValid(string name, string sku, decimal price, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidProductNameException();
        }
        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new InvalidSkuException();
        }
        if (price < 0)
        {
            throw new InvalidPriceException();
        }
        if (categoryId == Guid.Empty)
        {
            throw new CategoryReferenceIsRequiredException();
        }
    }
}
