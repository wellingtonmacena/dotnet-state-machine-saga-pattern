namespace WebApi.Inventory.Models
{
    public class Product: Entity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
    }
}
