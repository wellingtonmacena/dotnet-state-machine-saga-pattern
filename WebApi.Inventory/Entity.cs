namespace WebApi.Inventory;

public class Entity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get;  set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get;  set; } = DateTime.UtcNow;

    public void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
