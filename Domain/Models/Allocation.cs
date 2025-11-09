namespace WarehouseMS.Domain.Models;

public class Allocation
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid SKUId { get; private set; }
    public int Quantiy { get; private set; }

    public Allocation(Guid skuId, int quantity)
    {
        SKUId = skuId;
        Quantiy = quantity;
    }
}
