namespace WarehouseMS.Domain.Models;

public class SKU
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Product Product { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; }
    public int TotalQuantity { get; private set; }
    public int AllocatedQuantity { get; private set; }
    public int AvailableQuantity => TotalQuantity - AllocatedQuantity;

    public SKU(Product product, Location location, int totalQuantity)
    {
        Product = product;
        ProductId = product.Id;
        Location = location;
        LocationId = location?.Id ?? Guid.Empty;
        TotalQuantity = totalQuantity;
        AllocatedQuantity = 0;
    }

    public void Allocate(int quantity)
    {
        if (quantity > AvailableQuantity)
            return;

        AllocatedQuantity += quantity;
    }

    public void Release(int quantity)
    {
        AllocatedQuantity = Math.Max(0, AllocatedQuantity - quantity);
    }

    public void AdjustedQuantity(int difference)
    {
        TotalQuantity += difference;
    }
}