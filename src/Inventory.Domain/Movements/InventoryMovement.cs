namespace Inventory.Domain.Movements;
public sealed class InventoryMovement
{
    private InventoryMovement(Guid id, Guid productId, MovementType type, int quantity, DateTime occurredAtUtc, string notes)
    {
        Id = id;
        ProductId = productId;
        Type = type;
        Quantity = quantity;
        OccurredAtUtc = occurredAtUtc;
        Notes = notes;
    }
    public Guid Id { get; }
    public Guid ProductId { get; }
    public MovementType Type { get; }
    public int Quantity { get; }
    public DateTime OccurredAtUtc { get; }
    public string Notes { get; }
    public static InventoryMovement Create(NewInventoryMovement request)
    {
        if (request.ProductId == Guid.Empty)
        {
            throw new ProductReferenceIsRequiredException();
        }
        if (request.Quantity <= 0)
        {
            throw new QuantityMustBePositiveException();
        }
        if (!Enum.IsDefined(request.Type))
        {
            throw new InvalidMovementTypeException();
        }
        var notes = request.Notes ?? string.Empty;

        return new InventoryMovement(Guid.NewGuid(), request.ProductId, request.Type, request.Quantity, request.OccurredAtUtc, notes.Trim());
    }
}
