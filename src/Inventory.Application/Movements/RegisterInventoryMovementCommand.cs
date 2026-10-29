using Inventory.Application.Products;
using Inventory.Domain.Movements;
using MediatR;
namespace Inventory.Application.Movements;
public sealed class RegisterInventoryMovementCommand : IRequest<ProductResponse>
{
    public RegisterInventoryMovementCommand(Guid productId, MovementType type, int quantity, DateTime occurredAtUtc, string? notes)
    {
        ProductId = productId;
        Type = type;
        Quantity = quantity;
        OccurredAtUtc = occurredAtUtc;
        Notes = notes;
    }
    public Guid ProductId { get; }
    public MovementType Type { get; }
    public int Quantity { get; }
    public DateTime OccurredAtUtc { get; }
    public string? Notes { get; }
}
