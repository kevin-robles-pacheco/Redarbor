using Inventory.Domain.Movements;
using Inventory.Domain.Products;
namespace Inventory.Domain.Tests;
public sealed class ProductTests
{
    [Fact]
    public void Create_WithValidRequest_StartsWithZeroQuantity()
    {
        var createdAt = new DateTime(2026, 10, 28, 12, 0, 0, DateTimeKind.Utc);
        var categoryId = Guid.NewGuid();
        var request = new CreateProduct("  Chair  ", null, "  CHAIR-1  ", 19.5m, categoryId, createdAt);
        var product = Product.Create(request);
        Assert.Equal("Chair", product.Name);
        Assert.Equal(string.Empty, product.Description);
        Assert.Equal("CHAIR-1", product.Sku);
        Assert.Equal(19.5m, product.Price);
        Assert.Equal(categoryId, product.CategoryId);
        Assert.Equal(0, product.QuantityOnHand);
        Assert.True(product.IsActive);
    }
    [Fact]
    public void Create_WhenNameIsBlank_ThrowsInvalidProductNameException()
    {
        var request = ValidRequest(name: " ");
        Assert.Throws<InvalidProductNameException>(() => Product.Create(request));
    }
    [Fact]
    public void Create_WhenSkuIsBlank_ThrowsInvalidSkuException()
    {
        var request = ValidRequest(sku: " ");
        Assert.Throws<InvalidSkuException>(() => Product.Create(request));
    }
    [Fact]
    public void Create_WhenPriceIsNegative_ThrowsInvalidPriceException()
    {
        var request = ValidRequest(price: -1m);
        Assert.Throws<InvalidPriceException>(() => Product.Create(request));
    }
    [Fact]
    public void Create_WhenCategoryIdIsEmpty_ThrowsCategoryReferenceIsRequiredException()
    {
        var request = ValidRequest(categoryId: Guid.Empty);
        Assert.Throws<CategoryReferenceIsRequiredException>(() => Product.Create(request));
    }
    [Fact]
    public void Apply_WhenMovementBelongsToAnotherProduct_ThrowsMovementProductMismatchException()
    {
        var product = Product.Create(ValidRequest());
        var movement = InventoryMovement.Create(new NewInventoryMovement(Guid.NewGuid(), MovementType.In, 1, product.CreatedAtUtc, null));
        Assert.Throws<MovementProductMismatchException>(() => product.Apply(movement));
    }
    private static CreateProduct ValidRequest(string name = "Chair", string sku = "CHAIR-1", decimal price = 10m, Guid? categoryId = null)
    {
        var createdAt = new DateTime(2026, 10, 28, 12, 0, 0, DateTimeKind.Utc);

        return new CreateProduct(name, "Wood", sku, price, categoryId ?? Guid.NewGuid(), createdAt);
    }
}
