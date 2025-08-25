namespace WebApi.Orders
{
    public class Order: Entity
    {
        public Guid CartId  { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
