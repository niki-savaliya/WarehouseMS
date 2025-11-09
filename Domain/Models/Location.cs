namespace WarehouseMS.Domain.Models;

public class Location
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Code { get; private set; }
    public bool IsLocked { get; private set; }

    public Location(string code, bool isLocked = false)
    {
        Code = code;
        IsLocked = isLocked;
    }

    public void Lock()
    {
        IsLocked = true;
    }

    public void Unlock()
    {
        IsLocked = false;
    }
}
