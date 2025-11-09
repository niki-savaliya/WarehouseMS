using WarehouseMS.Application.Services;
using WarehouseMS.Domain;
using WarehouseMS.Domain.Models;

namespace WarehouseMS;

public class Program
{
    private static List<Product> _products = new();
    private static List<Location> _locations = new();
    private static List<SKU> _skus = new();
    private static List<Order> _orders = new();

    private static OrderRepository _orderRepo = null!;
    private static SkuRepository _skuRepo = null!;
    private static AllocationService _allocationService = null!;
    private static SkuCorrectionService _skuCorrectionService = null!;
    private static CancellationService _cancellationService = null!;

    public static void Main(string[] args)
    {
        SeedData();
        InitializeServices();

        Console.WriteLine("=== Warehouse Stock Allocation System ===");
        Console.WriteLine("1. View SKUs");
        Console.WriteLine("2. View Orders");
        Console.WriteLine("3. Allocate Released Orders");
        Console.WriteLine("4. Cancel Order");
        Console.WriteLine("5. Correct SKU Quantity");
        Console.WriteLine("6. Clear");
        Console.WriteLine("7. Exit");

        while (true)
        {
            Console.Write("\nChoose an Option - ");
            var choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    ShowSkus();
                    break;
                case "2":
                    ShowOrders();
                    break;
                case "3":
                    AllocateOrders();
                    break;
                case "4":
                    CancelOrder();
                    break;
                case "5":
                    CorrectSkuQuantity();
                    break;
                case "6":
                    Console.Clear();
                    Console.WriteLine("=== Warehouse Stock Allocation System ===");
                    Console.WriteLine("1. View SKUs");
                    Console.WriteLine("2. View Orders");
                    Console.WriteLine("3. Allocate Released Orders");
                    Console.WriteLine("4. Cancel Order");
                    Console.WriteLine("5. Correct SKU Quantity");
                    Console.WriteLine("6. Clear");
                    Console.WriteLine("7. Exit");
                    break;
                case "7":
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    private static void SeedData()
    {
        // Products
        var juice = new Product("Fruit Juice");
        var milk = new Product("Milk 1L");
        var bread = new Product("Whole Wheat Bread");
        _products.AddRange(new[] { juice, milk, bread });

        // Locations
        var loc1 = new Location("A-01", false);
        var loc2 = new Location("A-02", false);
        var loc3 = new Location("B-01", true);
        _locations.AddRange(new[] { loc1, loc2, loc3 });

        // SKUs
        var sku1 = new SKU(juice, loc1, 15);
        var sku2 = new SKU(juice, loc2, 5);
        var sku3 = new SKU(milk, loc1, 10);
        var sku4 = new SKU(bread, loc2, 25);
        _skus.AddRange(new[] { sku1, sku2, sku3, sku4 });

        // Orders
        var order1 = new Order(PriorityEnum.High, true);
        order1.AddLine(new OrderLine(juice, 10));

        // Adding a tiny delay to simulate FIFO ordering
        Thread.Sleep(1000);

        var order2 = new Order(PriorityEnum.High, true);
        order2.AddLine(new OrderLine(juice, 10));

        var order3 = new Order(PriorityEnum.Normal, false);
        order3.AddLine(new OrderLine(bread, 10));

        var order4 = new Order(PriorityEnum.Low, true);
        order4.AddLine(new OrderLine(milk, 15));

        _orders.AddRange(new[] { order1, order2, order3, order4 });
    }


    private static void InitializeServices()
    {
        _orderRepo = new OrderRepository(_orders);
        _skuRepo = new SkuRepository(_skus);
        _allocationService = new AllocationService(_orderRepo, _skuRepo);
        _skuCorrectionService = new SkuCorrectionService(_skuRepo, _orderRepo);
        _cancellationService = new CancellationService(_orderRepo, _skuRepo);
    }

    private static void ShowSkus()
    {
        Console.WriteLine("=== SKUs ===");
        foreach (var sku in _skus)
        {
            Console.WriteLine($"SKU {sku.Id} | Product: {sku.Product.Name} - {sku.ProductId} | Loc: {sku.Location.Code} | " +
                              $"Available: {sku.AvailableQuantity}/{sku.TotalQuantity} | Locked: {sku.Location.IsLocked}");
        }
    }

    private static void ShowOrders()
    {
        Console.WriteLine("=== Orders ===");
        foreach (var order in _orders)
        {
            Console.WriteLine($"\nOrder {order.Id} | Priority: {order.Priority} | CompleteDelivery: {order.CompleteDeliveryRequired}" +
                                $" | Ready: {order.IsReadyToDeliver}");
            foreach (var line in order.OrderLines)
            {
                Console.WriteLine($"\tLine {line.Id} | Product: {line.Product.Name} - {line.ProductId} | Qty: {line.RemainingQuantity}");
                foreach (var alloc in line.Allocations)
                {
                    Console.WriteLine($"\t\tAllocated from SKU {alloc.SKUId} -> {alloc.Quantiy}");
                }
            }
        }
    }

    private static void AllocateOrders()
    {
        _allocationService.AllocateReleasedOrders();
        Console.WriteLine("Allocation complete!");
    }

    private static void CancelOrder()
    {
        Console.Write("Enter Order ID to cancel: ");
        var input = Console.ReadLine();

        if (Guid.TryParse(input, out var orderId))
            _cancellationService.CancelOrder(orderId);
        else
            Console.WriteLine("Invalid ID format.");
    }

    private static void CorrectSkuQuantity()
    {
        Console.Write("Enter SKU ID to correct: ");
        var input = Console.ReadLine();

        if (Guid.TryParse(input, out var skuId))
        {
            Console.Write("Enter new total quantity: ");
            if (int.TryParse(Console.ReadLine(), out var newQty))
                _skuCorrectionService.CorrectSkuQuantity(skuId, newQty);
            else
                Console.WriteLine("Invalid number.");
        }
        else
            Console.WriteLine("Invalid ID format.");
    }
}
