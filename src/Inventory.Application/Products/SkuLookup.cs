namespace Inventory.Application.Products;
public sealed class SkuLookup
{
    public SkuLookup(string sku, Guid? productId)
    {
        Sku = sku;
        ProductId = productId;
    }
    public string Sku { get; }
    public Guid? ProductId { get; }
}
