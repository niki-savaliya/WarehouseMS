namespace WarehouseMS.Domain.Models;

public class Order
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public PriorityEnum Priority { get; private set; }
    public bool CompleteDeliveryRequired { get; private set; }
    public bool IsCancelled { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public List<OrderLine> OrderLines { get; private set; } = new();

    public bool IsReadyToDeliver => OrderLines.All(x => x.RemainingQuantity == 0);

    public Order(PriorityEnum priority, bool completeDeliveryRequired)
    {
        Priority = priority;
        CompleteDeliveryRequired = completeDeliveryRequired;
        IsCancelled = false;
    }

    public void AddLine(OrderLine orderLine)
    {
        OrderLines.Add(orderLine);
    }

    public List<(Guid skuId, int quantity)> Cancel()
    {
        if (IsCancelled)
            return new List<(Guid, int)>();

        var allocationsToRelease = new List<(Guid skuId, int quantity)>();

        foreach (var line in OrderLines)
        {
            foreach (var allocation in line.Allocations)
            {
                allocationsToRelease.Add((allocation.SKUId, allocation.Quantiy));
            }

            line.ClearAllocations();
            line.Cancel();
        }

        IsCancelled = true;
        return allocationsToRelease;
    }

    public List<(Guid skuId, int quantity)> CancelLine(Guid orderLineId)
    {
        var allocationsToRelease = new List<(Guid skuId, int quantity)>();

        var line = OrderLines.FirstOrDefault(ol => ol.Id == orderLineId);
        if (line != null && !line.IsCancelled)
        {
            foreach (var allocation in line.Allocations)
            {
                allocationsToRelease.Add((allocation.SKUId, allocation.Quantiy));
            }

            line.ClearAllocations();
            line.Cancel();
        }

        if (OrderLines.All(x => x.IsCancelled))
            IsCancelled = true;

        return allocationsToRelease;
    }
}