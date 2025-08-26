namespace WebApi.Inventory.Models
{
    public class StockItem : Entity
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }  // navigation
        public int Quantity { get; set; }
        public Guid StorageId { get; set; }   // 👈 precisa disso
        public Storage Storage { get; set; }
        public string Location { get; set; } = string.Empty; // opcional (ex: prateleira, depósito)
    }
}
