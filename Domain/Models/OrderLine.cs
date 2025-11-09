namespace WarehouseMS.Domain.Models;

public class OrderLine
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; }
    public int RequestedQuantity { get; private set; }
    public List<Allocation> Allocations { get; private set; } = new();
    public bool IsCancelled { get; private set; } = false;

    public int AllocatedQuantity => Allocations.Sum(a => a.Quantiy);
    public int RemainingQuantity => RequestedQuantity - AllocatedQuantity;

    public OrderLine(Product product, int requestedQuantity)
    {
        Product = product;
        ProductId = product.Id;
        RequestedQuantity = requestedQuantity;
    }

    public void AddAllocation(Guid skuId, int quantity)
    {
        Allocations.Add(new Allocation(skuId, quantity));
    }

    public void ClearAllocations()
    {
        Allocations.Clear();
    }

    public void Cancel()
    {
        IsCancelled = true;
    }
}