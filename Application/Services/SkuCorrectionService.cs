using WarehouseMS.Application.Services.Interfaces;
using WarehouseMS.Domain.Models;

namespace WarehouseMS.Application.Services;

public class SkuCorrectionService
{
    private readonly ISkuRepository _skuRepository;
    private readonly IOrderRepository _orderRepository;

    public SkuCorrectionService(ISkuRepository skuRepository, IOrderRepository orderRepository)
    {
        _skuRepository = skuRepository;
        _orderRepository = orderRepository;
    }

    public void CorrectSkuQuantity(Guid skuId, int newTotalQuantity)
    {
        var sku = _skuRepository.GetSkuById(skuId);
        if (sku == null)
        {
            Console.WriteLine($"SKU {skuId} not found.");
            return;
        }

        int oldTotal = sku.TotalQuantity;
        int difference = newTotalQuantity - oldTotal;

        Console.WriteLine($"=== SKU Quantity Correction ===");
        Console.WriteLine($"SKU: {sku.Id}");
        Console.WriteLine($"Location: {sku.Location.Code}");
        Console.WriteLine($"Old Total: {oldTotal}");
        Console.WriteLine($"New Total: {newTotalQuantity}");
        Console.WriteLine($"Currently Allocated: {sku.AllocatedQuantity}");

        sku.AdjustedQuantity(difference);

        Console.WriteLine($"New Available: {sku.AvailableQuantity}");

        if (sku.AvailableQuantity < 0)
        {
            Console.WriteLine($"WARNING: Available quantity is negative!");
            Console.WriteLine($"Shortage: {Math.Abs(sku.AvailableQuantity)}");
            HandleNegativeAvailability(sku);
        }
        else if (difference > 0)
            Console.WriteLine($"Positive correction - No issues.");
        else
            Console.WriteLine($"Negative correction - But still enough stock.");
    }

    private void HandleNegativeAvailability(SKU sku)
    {
        var affectedOrders = _orderRepository.GetAllOrders().Where(o => !o.IsCancelled && o.OrderLines.Any(ol => ol.Allocations.Any(a => a.SKUId == sku.Id))).ToList();

        Console.WriteLine($"Found {affectedOrders.Count} affected order(s)");

        foreach (var order in affectedOrders)
        {
            var affectedLines = order.OrderLines.Where(ol => ol.Allocations.Any(a => a.SKUId == sku.Id)).ToList();

            foreach (var line in affectedLines)
            {
                var allocationFromThisSku = line.Allocations.FirstOrDefault(a => a.SKUId == sku.Id);
                if (allocationFromThisSku == null) continue;

                int shortage = Math.Abs(sku.AvailableQuantity);

                var substituteSkus = _skuRepository.GetAvailableSkuForProduct(line.ProductId).Where(s => s.Id != sku.Id && s.AvailableQuantity > 0).ToList();

                bool canFindSubstitute = substituteSkus.Sum(s => s.AvailableQuantity) >= shortage;

                if (canFindSubstitute)
                {
                    int remaining = shortage;
                    foreach (var sub in substituteSkus)
                    {
                        if (remaining <= 0) break;
                        int qty = Math.Min(remaining, sub.AvailableQuantity);
                        sub.Allocate(qty);
                        line.AddAllocation(sub.Id, qty);
                        remaining -= qty;
                        Console.WriteLine($"\tAllocated {qty} from substitute SKU {sub.Id}");
                    }
                }
                else
                {
                    if (order.CompleteDeliveryRequired)
                    {
                        Console.WriteLine($"\tOrder {order.Id} requires complete delivery - deallocating all");
                        foreach (var ol in order.OrderLines)
                        {
                            foreach (var alloc in ol.Allocations.ToList())
                            {
                                var skuToRelease = _skuRepository.GetSkuById(alloc.SKUId);
                                if (skuToRelease != null)
                                    skuToRelease.Release(alloc.Quantiy);
                            }
                            ol.ClearAllocations();
                        }
                        break;
                    }
                    else
                        Console.WriteLine($"\tPartial delivery OK - keeping allocations");
                }
            }
        }
    }
}