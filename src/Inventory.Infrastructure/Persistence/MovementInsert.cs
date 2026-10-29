using Inventory.Domain.Movements;
namespace Inventory.Infrastructure.Persistence;
public sealed class MovementInsert
{
    public MovementInsert(InventoryMovement movement)
    {
        Id = movement.Id;
        ProductId = movement.ProductId;
        Type = (int)movement.Type;
        Quantity = movement.Quantity;
        OccurredAtUtc = movement.OccurredAtUtc;
        Notes = movement.Notes;
    }
    public Guid Id { get; }
    public Guid ProductId { get; }
    public int Type { get; }
    public int Quantity { get; }
    public DateTime OccurredAtUtc { get; }
    public string Notes { get; }
}
