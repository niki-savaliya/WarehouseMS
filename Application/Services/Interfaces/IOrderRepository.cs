using WarehouseMS.Domain.Models;

namespace WarehouseMS.Application.Services.Interfaces;

public interface IOrderRepository
{
    IEnumerable<Order> GetReleasedOrders();
    Order GetOrderById(Guid id);
    IEnumerable<Order> GetAllOrders();
}