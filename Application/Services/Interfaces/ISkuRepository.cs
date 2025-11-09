using WarehouseMS.Domain.Models;

namespace WarehouseMS.Application.Services.Interfaces;

public interface ISkuRepository
{
    IEnumerable<SKU> GetAvailableSkuForProduct(Guid productId);
    SKU GetSkuById(Guid id);
}
