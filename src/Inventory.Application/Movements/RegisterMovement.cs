using Inventory.Domain.Movements;
using Inventory.Domain.Products;
namespace Inventory.Application.Movements;
public sealed class RegisterMovement
{
    public RegisterMovement(Product product, InventoryMovement movement)
    {
        Product = product;
        Movement = movement;
    }
    public Product Product { get; }
    public InventoryMovement Movement { get; }
}
