namespace WebApi.Inventory.Models
{
    public class Storage : Entity
    {
        public ICollection<StockItem> Items { get; set; } = new List<StockItem>();
    }
}
