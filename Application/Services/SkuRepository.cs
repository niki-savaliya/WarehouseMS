using WarehouseMS.Application.Services.Interfaces;
using WarehouseMS.Domain.Models;

namespace WarehouseMS.Application.Services;

public class SkuRepository : ISkuRepository
{
    private readonly List<SKU> _skus;
    public static SkuRepository Instance { get; set; }
    public SkuRepository(List<SKU> skus)
    {
        _skus = skus;
        Instance = this;
    }

    public IEnumerable<SKU> GetAvailableSkuForProduct(Guid productId)
        => _skus.Where(sku => sku.ProductId == productId && sku.AvailableQuantity > 0 && sku.Location != null && !sku.Location.IsLocked);

    public SKU GetSkuById(Guid id) => _skus.FirstOrDefault(sku => sku.Id == id);
}
