namespace WarehouseMS.Domain.Models;

public class Product
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;

    public Product(string name)
    {
        Name = name;
    }
}
