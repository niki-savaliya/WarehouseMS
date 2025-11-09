using WarehouseMS.Application.Services.Interfaces;
using WarehouseMS.Domain.Models;

namespace WarehouseMS.Application.Services;

public class OrderRepository : IOrderRepository
{
    private readonly List<Order> _orders;

    public OrderRepository(List<Order> orders)
    {
        _orders = orders;
    }

    public IEnumerable<Order> GetReleasedOrders()
        => _orders.Where(o => !o.IsCancelled && !o.IsReadyToDeliver);

    public Order GetOrderById(Guid id) => _orders.FirstOrDefault(o => o.Id == id);

    public IEnumerable<Order> GetAllOrders()
        => _orders;
}