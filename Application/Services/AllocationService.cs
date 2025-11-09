using WarehouseMS.Application.Services.Interfaces;

namespace WarehouseMS.Application.Services;

public class AllocationService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ISkuRepository _skuRepository;

    public AllocationService(IOrderRepository orderRepository, ISkuRepository skuRepository)
    {
        _orderRepository = orderRepository;
        _skuRepository = skuRepository;
    }

    public void AllocateReleasedOrders()
    {
        var orders = _orderRepository.GetReleasedOrders()
                                   .OrderByDescending(o => o.Priority)
                                   .ThenBy(o => o.CreatedAt)
                                   .ToList();

        foreach (var order in orders)
        {
            bool canFullyAllocate = true;

            var allocationsPerformed = new List<(Guid skuId, int qty)>();

            foreach (var line in order.OrderLines)
            {
                int requiredQty = line.RemainingQuantity;

                if (requiredQty <= 0)
                    continue;

                var availableSkus = _skuRepository.GetAvailableSkuForProduct(line.ProductId)
                                                  .OrderBy(s => s.LocationId)
                                                  .ToList();

                foreach (var sku in availableSkus)
                {
                    if (requiredQty <= 0)
                        break;

                    var allocQty = Math.Min(requiredQty, sku.AvailableQuantity);
                    if (allocQty <= 0)
                        continue;

                    sku.Allocate(allocQty);
                    line.AddAllocation(sku.Id, allocQty);
                    allocationsPerformed.Add((sku.Id, allocQty));
                    requiredQty -= allocQty;
                }

                if (requiredQty > 0 && order.CompleteDeliveryRequired)
                {
                    canFullyAllocate = false;
                    break;
                }
            }

            if (!canFullyAllocate && order.CompleteDeliveryRequired)
            {
                foreach (var ol in order.OrderLines)
                {
                    foreach (var a in ol.Allocations.ToList())
                    {
                        var sku = _skuRepository.GetSkuById(a.SKUId);
                        if (sku != null)
                        {
                            sku.Release(a.Quantiy);
                        }
                    }

                    ol.ClearAllocations();
                }
            }
        }
    }
}
