using WarehouseMS.Application.Services.Interfaces;

namespace WarehouseMS.Application.Services;

public class CancellationService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ISkuRepository _skuRepository;

    public CancellationService(IOrderRepository orderRepository, ISkuRepository skuRepository)
    {
        _orderRepository = orderRepository;
        _skuRepository = skuRepository;
    }

    public void CancelOrder(Guid orderId)
    {
        var order = _orderRepository.GetOrderById(orderId);
        if (order == null)
        {
            Console.WriteLine($"Order {orderId} not found.");
            return;
        }

        if (order.IsCancelled)
        {
            Console.WriteLine($"Order {orderId} is already cancelled.");
            return;
        }

        Console.WriteLine($"=== Cancelling Order {orderId} ===");

        var allocationsToRelease = order.Cancel();

        foreach (var (skuId, quantity) in allocationsToRelease)
        {
            var sku = _skuRepository.GetSkuById(skuId);
            if (sku != null)
            {
                sku.Release(quantity);
                Console.WriteLine($"Released {quantity} units back to SKU {skuId} at {sku.Location.Code}");
            }
        }

        Console.WriteLine($"Order cancelled successfully! Total {allocationsToRelease.Count} allocation(s) released.");
    }

    public void CancelOrderLine(Guid orderId, Guid orderLineId)
    {
        var order = _orderRepository.GetOrderById(orderId);
        if (order == null)
        {
            Console.WriteLine($"Order {orderId} not found.");
            return;
        }

        Console.WriteLine($"=== Cancelling Order Line {orderLineId} ===");

        var allocationsToRelease = order.CancelLine(orderLineId);

        foreach (var (skuId, quantity) in allocationsToRelease)
        {
            var sku = _skuRepository.GetSkuById(skuId);
            if (sku != null)
            {
                sku.Release(quantity);
                Console.WriteLine($"Released {quantity} units back to SKU {skuId} at {sku.Location.Code}");
            }
        }

        Console.WriteLine($"Order line cancelled! Total {allocationsToRelease.Count} allocation(s) released.");

        if (order.IsCancelled)
            Console.WriteLine($" All lines cancelled - Order {orderId} is now fully cancelled.");
    }
}