namespace Inventory.Api.Contracts;
public sealed class RegisterInventoryMovementRequest
{
    public Guid ProductId { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Notes { get; set; }
}
